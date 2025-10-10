using HolyWar.Units;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class DamageEffect : SpellEffect
{
    [SerializeField] private int _baseDamageValue;

    public override IEnumerator Effect(BaseUnit target, int spellPower)
    {
        //Нанесение цели _damageValue урона
        target.TakeDamage(_baseDamageValue + spellPower);
        //Это метод с мгновенным эффектом, поэтому нам не нужно ничего ждать. Но рутина в юнити ждёт, что мы явно об этом скажем.
        yield break;
    }
}