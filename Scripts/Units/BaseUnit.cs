using HolyWar.Fields;
using HolyWar.FloatText;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace HolyWar.Units
{
    public class BaseUnit : MonoBehaviour
    {
        public BaseStats Stats;

        [SerializeField] private int CurrentHealth;
        [SerializeField] private int CurrentMana;

        [SerializeField] private List<SpellSO> Spells = new();

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
        public Action<BaseUnit> OnHealthChanged;

        private bool _isDead = false;

        public bool IsDead { get { return _isDead; } }

        public int GetCurrentHealth()
        { 
            return CurrentHealth;
        }

        public void TakeHeal(int heal)
        {
            if (!_isDead)
            {
                CurrentHealth += heal;
                if (CurrentHealth > Stats.MaxHealth)
                    CurrentHealth = Stats.MaxHealth;

                Debug.Log($"{name} recieving {heal} heal, remain {CurrentHealth} hp");
                FloatingTextSystem.CreateFloatingText(transform.position, "+ " + heal.ToString(), FloatingTextSystem.TextColors.Green);

                _healthBar.SetHealth(CurrentHealth, Stats.MaxHealth);
                OnHealthChanged.Invoke(this);
            }
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            Debug.Log($"{name} taking {damage} damage, remain {CurrentHealth} hp");
            FloatingTextSystem.CreateFloatingText(transform.position, "- " + damage.ToString(), FloatingTextSystem.TextColors.Red);

            _healthBar.SetHealth(CurrentHealth, Stats.MaxHealth);

            if (CurrentHealth <= 0 && _isDead == false)
            {
                //Нам нужно отдельно объявить isDead чтобы запоздавшие юниты, которые ещё атакуют нашу цель, не запустили метод вновь.
                _isDead = true;
                StartDying();
                return;
            }

            OnHealthChanged.Invoke(this);
        }

        private void StartDying()
        {
            _isDying = true;
            _dyingFrameCounter = _amountOfDyingFrames;

            _healthBar.HideHealtbar();

            OnKilled.Invoke(this);
            Debug.Log($"Unit {gameObject.name} is starting dying");

            //Скроем объект
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void DieCompletely()
        {
            //Это нуджно дл ситуаций, когда два юнита атакуют одновременно, и хоть технически это невозможно
            //по правилам игры было бы справедливо, чтобы юнит успел за пару последний фреймов своей жизни првоести свою атаку.
            Debug.Log($"Unit {gameObject.name} is dead completely");

            OnKilled = null;
            OnHealthChanged = null;

            //Остановим атаку
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

        //Главная проблема - понять куда именно колдовать спелл. Для этого у самого спелла есть переменные на определение полей и на определение того, должны лы они быть дружественными
        //или вражескими.
        private void CastSpell(SpellSO spell)
        {
            if (spell.IsFriendly)
            {
                //Смерть всего хорошего, ждём нормальное лобби
                var currentPlayerNumber = Math.Abs(OppositePlayerNumber - 1);
                foreach (var accordingField in spell.FieldPriority)
                {
                    var friendlyField = _battleManager.GetField(currentPlayerNumber, accordingField);

                }
            }
            else
            {

            }
        }

        private void BattleStartListener()
        {
            CurrentHealth = Stats.MaxHealth;
            CurrentMana = Stats.MaxMana;
            _isDying = false;
            _isDead = false;

            _healthBar.ShowHealtbar();
            _healthBar.SetHealth(CurrentHealth, Stats.MaxHealth);

            //Поставим все заклинания на реуглярный каст
            foreach (var spell in Spells)
            {
                CastSpell(spell);
            }

            _battleManager = FindAnyObjectByType<BattleManager>();
            _battleManager.AddUnitToTeam(Mathf.Abs(OppositePlayerNumber - 1));
            BattleProcess(null);
        }

        private void BattleEndListener()
        {
            _healthBar.HideHealtbar();
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
                //Начинаем бить юнита по кулдауну.
                InvokeRepeating(nameof(InflictDamage), 0f, Stats.AttackSpeed);
            }
            else
            {
                //Если прошлись по всем полям и не нашли подходящих - сидим кукуем.
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
            if (_target == null || _target.IsDead)
            {
                //Если наша текущая цель мертва или не найдена - ищем новую
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