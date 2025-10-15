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
                    Debug.LogWarning($"В поле приоритета полей не должно быть дублей");
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
            //Пройдемся по всем полям в порядке приоритета и посмотрим, есть ли у них подходящие для каста юниты

            Player targetPlayer = casterPlayer;
            //Если заклинание враждебное, то цель - оппонент кастующего
            if (!IsFriendly)
            {
                targetPlayer = battleManager.GetOpponent(casterPlayer);
            }

            var fieldUnits = battleManager.GetFieldUnits(targetPlayer, fieldType).Item1;

            //Отфильтруем найденных юнитов по заданным параметрам
            if (TargetFilter.IsFindTarget(fieldUnits, out List<BaseUnit> targetUnits))
            {
                //Применим эффект к каждому найденному юниту
                foreach (var target in targetUnits)
                {
                    var spellEffectRunner = target.GetOrAddComponent<SpellEffectRunner>();
                    spellEffectRunner.RegisterSpell(SpellEffects, spellPower, target);
                }

                //Эффект заклинания выполнен, прерываем цикл
                return true;
            }
        }
        Debug.Log($"Didn't find targets for spell {this.name}");
        return false;
    }



}
