using HolyWar.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class DamagePerTimeEffect : SpellEffect
{
    [SerializeField] private int _baseDamageValue;
    [SerializeField] private int _ticksAmount;
    [SerializeField] private int _tickInterval;

    public override IEnumerator Effect(BaseUnit target, int spellPower)
    {
        for (int i = 0; i < _ticksAmount; i++)
        {
            //Наносим урон с константой спелл павера
            target.TakeDamage(_baseDamageValue + spellPower);
            //Встаём на ожидание следующего тика
            yield return new WaitForSeconds(_tickInterval);
        }
    }
}