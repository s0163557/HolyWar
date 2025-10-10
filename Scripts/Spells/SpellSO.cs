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
                    Debug.LogWarning($"В поле приоритета полей не должно быть дублей");
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
            //Пройдемся по всем полям в порядке приоритета и посмотрим, есть ли у них подходящие для каста юниты
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
                return;
            }
        }
    }



}
