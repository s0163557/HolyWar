using HolyWar.Fields;
using HolyWar.FloatText;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

namespace HolyWar.Units
{
    public class BaseUnit : MonoBehaviour
    {
        public BaseStats Stats;
        [SerializeField] public int CurrentHealth { private set; get; }
        [SerializeField] private int CurrentMana;

        [SerializeField] private List<SpellSO> Spells = new();

        [Flags]
        public enum UnitState
        {
            None = 0,
            Damaged = 1 << 1,
            Dead = 1 << 2,
        }

        public UnitState States { private set; get; }

        public int OppositePlayerNumber;
        public string UnitName;
        private BattleManager _battleManager;
        private BaseUnit _target;


        private HealthBar _healthBar;

        protected float xCoord;
        public float XCoord
        {
            get
            {
                return xCoord;
            }
            set
            {
                xCoord = value;
            }
        }

        public Action<BaseUnit> OnKilled;


        public void TakeHeal(int heal)
        {
            //���� ���� �� ���� - ��������� ����
            if ((States & UnitState.Dead) == 0)
            {
                CurrentHealth += heal;
                if (CurrentHealth >= Stats.MaxHealth)
                {
                    CurrentHealth = Stats.MaxHealth;
                    //����� ������ ���� ��� ���� ����������
                    States &= ~UnitState.Damaged;
                }

                Debug.Log($"{name} recieving {heal} heal, remain {CurrentHealth} hp");
                FloatingTextSystem.CreateFloatingText(transform.position, "+ " + heal.ToString(), FloatingTextSystem.TextColors.Green);

                _healthBar.SetHealth(CurrentHealth, Stats.MaxHealth);
            }
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            //������� ������ ����, ��� ���� ����������
            States |= UnitState.Damaged;
            Debug.Log($"{name} taking {damage} damage, remain {CurrentHealth} hp");
            FloatingTextSystem.CreateFloatingText(transform.position, "- " + damage.ToString(), FloatingTextSystem.TextColors.Red);

            _healthBar.SetHealth(CurrentHealth, Stats.MaxHealth);

            if (CurrentHealth <= 0 && (States & UnitState.Dead) == 0)
            {
                //��� ����� �������� �������� ������ Dead ����� ����������� �����, ������� ��� ������� ���� ����, �� ��������� ����� �����.
                States |= UnitState.Dead;
                StartDying();
            }
        }

        private void StartDying()
        {
            StopAllCoroutines();
            _isDying = true;
            _dyingFrameCounter = _amountOfDyingFrames;

            _healthBar.HideHealtbar();

            OnKilled.Invoke(this);
            Debug.Log($"Unit {gameObject.name} is starting dying");

            //������ ������
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void DieCompletely()
        {
            //��� ������ �� ��������, ����� ��� ����� ������� ������������, � ���� ���������� ��� ����������
            //�� �������� ���� ���� �� �����������, ����� ���� ����� �� ���� ��������� ������� ����� ����� �������� ���� �����.
            Debug.Log($"Unit {gameObject.name} is dead completely");

            //��������� �����
            CancelInvoke(nameof(InflictDamage));
            _battleManager.RemoveUnitFromTeam(Mathf.Abs(OppositePlayerNumber - 1));
        }

        private void Start()
        {
            CurrentHealth = Stats.MaxHealth;
            CurrentMana = Stats.MaxMana;
            _dyingFrameCounter = _amountOfDyingFrames;

            _healthBar = this.AddComponent<HealthBar>();
            _healthBar.Initialize(gameObject.transform);
            _healthBar.HideHealtbar();

            Debug.Log($"Healthbar of unit {gameObject.name} initialized");

            EventBus.Subscribe(EventBus.EventsEnum.BattleStart, BattleStartListener);
            EventBus.Subscribe(EventBus.EventsEnum.BattleEnd, BattleEndListener);
        }

        //������� �������� - ������ ���� ������ ��������� �����. ��� ����� � ������ ������ ���� ���������� �� ����������� ����� � �� ����������� ����, ������ �� ��� ���� ��������������
        //��� ����������.
        private System.Collections.IEnumerator CastSpell(SpellSO spell)
        {
            while (true)
            {
                //��������, ������� �� ���� �� ���� ������
                if (CurrentMana >= spell.ManaCost)
                {
                    Debug.Log($"Unit {name} using spell {spell.name}");
                    if (spell.Cast(Math.Abs(OppositePlayerNumber - 1), Stats.SpellPower, _battleManager))
                    {
                        //���� ���� ������ �������, �������� ���� � ��������� ������������ �������
                        CurrentMana -= spell.ManaCost;
                        yield return new WaitForSeconds(spell.Cooldown);
                    }
                    else
                    {
                        //���� ���� �� ��������, ������ ��������� �������� ����� ����� �������
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    //���� ���� ���, �� ����� �� ����� ������ ��������, ����� ���-�� � ��������� � ���.
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        private void BattleStartListener()
        {
            _battleManager = FindAnyObjectByType<BattleManager>();

            CurrentHealth = Stats.MaxHealth;
            _isDying = false;
            States = UnitState.None;

            _healthBar.ShowHealtbar();
            _healthBar.SetHealth(CurrentHealth, Stats.MaxHealth);

            //�������� ��� ���������� �� ���������� ����
            foreach (var spell in Spells)
            {
                StartCoroutine(CastSpell(spell));
            }

            _battleManager.AddUnitToTeam(Mathf.Abs(OppositePlayerNumber - 1));
            BattleProcess(null);
        }

        private void BattleEndListener()
        {
            //����������� ���� ����� ��������� ���, ���� ���� ���������� ��������� �� ����� ������������
            CurrentMana += Stats.ManaRegeneration;
            _healthBar.HideHealtbar();
            StopAllCoroutines();
            CancelInvoke(nameof(InflictDamage));
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }

#nullable enable
        private void BattleProcess(BaseUnit? sender)
        {
            if (sender != null)
            {
                Debug.Log($"Unit {gameObject.name} change target from {sender.gameObject.name}");
                CancelInvoke(nameof(InflictDamage));
            }

            if (FindTarget(out var target))
            {
                _target = target!;
                Debug.Log($"Unit {gameObject.name} found {_target.gameObject.name}");
                //�������� ���� ����� �� ��������.
                InvokeRepeating(nameof(InflictDamage), 0f, Stats.AttackSpeed);
            }
            else
            {
                //���� �������� �� ���� ����� � �� ����� ���������� - ����� ������.
                Debug.Log($"Unit {gameObject.name} didnt find any target to attack, start idling");
                CancelInvoke(nameof(InflictDamage));
                _target = null;
            }
        }

        private bool FindTarget(out BaseUnit? target)
        {
            for (int i = (int)Stats.MinAttackRange; i <= (int)Stats.MaxAttackRange; i++)
            {
                var unitsListAndCount = _battleManager.GetField(OppositePlayerNumber, (FieldRange)i);
                if (unitsListAndCount.Item2 > 0)
                {
                    int randomUnitIndex = UnityEngine.Random.Range(0, unitsListAndCount.Item2);
                    target = unitsListAndCount.Item1.ElementAt(randomUnitIndex);
                    return true;
                }
            }

            target = null;
            return false;
        }

        private void InflictDamage()
        {
            if (_target == null || (_target.States & UnitState.Dead) != 0)
            {
                //���� ���� ������� ���� ������ ��� �� ������� - ���� �����
                CancelInvoke(nameof(InflictDamage));
                BattleProcess(null);
            }
            else
            {
                var damageValue = UnityEngine.Random.Range(Stats.MinAttackValue, Stats.MaxAttackValue + 1);
                Debug.Log($"Unit {gameObject.name} inflicted {damageValue} damage to {_target.gameObject.name}");
                _target.TakeDamage(damageValue);
            }
        }

        bool _isDying = false;
        int _amountOfDyingFrames = 5;
        int _dyingFrameCounter;
        // Update is called once per frame
        void Update()
        {
            if (_isDying)
            {
                _dyingFrameCounter--;
                if (_dyingFrameCounter <= 0)
                {
                    _isDying = false;
                    _dyingFrameCounter = _amountOfDyingFrames;
                    DieCompletely();
                }
            }
        }
    }
}