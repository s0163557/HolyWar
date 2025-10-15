using Assets.Scripts.Spells.Effects;
using HolyWar.Diplomacy;
using HolyWar.Fields;
using HolyWar.Units;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class SpellSO : ScriptableObject
{
    public string SpellName;
    public bool Icon;
    public int ManaCost;
    public float Cooldown;
    public bool IsFriendly;
    [SerializeReference] public TargetFilter TargetFilter;
    public List<FieldRange> FieldPriority = new(3);
    [SerializeReference] public List<SpellEffect> SpellEffects = new();


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

    public bool Cast(Player casterPlayer, int spellPower, BattleManager battleManager)
    {
        foreach (var fieldType in FieldPriority)
        {
            //��������� �� ���� ����� � ������� ���������� � ���������, ���� �� � ��� ���������� ��� ����� �����

            Player targetPlayer = casterPlayer;
            //���� ���������� ����������, �� ���� - �������� ����������
            if (!IsFriendly)
            {
                targetPlayer = battleManager.GetOpponent(casterPlayer);
            }

            var fieldUnits = battleManager.GetFieldUnits(targetPlayer, fieldType).Item1;

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
                return true;
            }
        }
        Debug.Log($"Didn't find targets for spell {this.name}");
        return false;
    }



}
