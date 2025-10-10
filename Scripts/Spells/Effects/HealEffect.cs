using HolyWar.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class HealEffect : SpellEffect
{
    [SerializeField] private int _baseHealValue;

    public override IEnumerator Effect(BaseUnit target, int spellPower)
    {
        //Излечение цели на _baseHealValue хитпоинтов.
        target.TakeHeal(_baseHealValue + spellPower);
        //Это метод с мгновенным эффектом, поэтому нам не нужно ничего ждать. Но рутина в юнити ждёт, что мы явно об этом скажем.
        yield break;
    }
}
