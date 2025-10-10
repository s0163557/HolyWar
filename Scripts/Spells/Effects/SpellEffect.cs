using HolyWar.Units;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class SpellEffect
{
    //Метод, который описывает эффект, который будем применён к цели.
    public abstract IEnumerator Effect(BaseUnit target, int spellPower);

}