using Assets.Scripts.Spells.Effects;
using HolyWar.Fields;
using HolyWar.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class SpellSO : ScriptableObject
{
    public bool Icon;
    public int ManaCost;
    public int Cooldown;
    public bool IsFriendly;
    public TargetFilter TargetFilter;
    public List<FieldRange> FieldPriority = new();
    [SerializeReference] public List<SpellEffect> SpellEffects = new();

    private BattleManager _battleManager;

    public void Init(BattleManager bm)
    { 
        _battleManager = bm;
    }

    private void OnValidate()
    {
        for (int i = FieldPriority.Count - 1; i >= 0; i--)
        {
            var current = FieldPriority[i];
            for (int j = 0; j < i; j++)
            {
                if (current == FieldPriority[j])
                {
                    Debug.LogWarning($"� ���� ���������� ����� �� ������ ���� ������");
                    FieldPriority[i] = 0;
                    break;
                }
            }
        }
    }

    public void Cast(int casterPlayerNumber, int spellPower)
    {
        foreach (var fieldType in FieldPriority)
        {
            //��������� �� ���� ����� � ������� ���������� � ���������, ���� �� � ��� ���������� ��� ����� �����
            int playerNumberTarget;
            if (TargetFilter.IsFriendly)
            {
                playerNumberTarget = casterPlayerNumber;
            }
            else
            { 
                playerNumberTarget = Math.Abs(casterPlayerNumber - 1);
            }
            var fieldUnits = _battleManager.GetField(playerNumberTarget, fieldType).Item1;

            //����������� ��������� ������ �� �������� ����������
            if (TargetFilter.IsFindTarget(fieldUnits, out List<BaseUnit> targetUnits))
            {
                //�������� ������ � ������� ���������� �����
                foreach (var target in targetUnits)
                {
                    var spellEffectRunner = target.GetOrAddComponent<SpellEffectRunner>();
                    spellEffectRunner.RegisterSpell(SpellEffects, spellPower, target);
                }

                //������ ���������� ��������, ��������� ����
                return;
            }
        }
    }



}
