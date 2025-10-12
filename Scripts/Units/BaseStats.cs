using System.Collections.Generic;
using HolyWar.Fields;
using UnityEngine;
using static HolyWar.Units.Enums;

namespace HolyWar.Units
{
    [CreateAssetMenu]
    public class BaseStats : ScriptableObject
    {
        public int MaxHealth;

        public int SpellPower;
        public int MaxMana;
        public int ManaRegeneration;

        public int MinAttackValue;
        public int MaxAttackValue;
        public float AttackSpeed;
        public AttackType AttackType;

        //То, какие поля юнит может атаковать
        public FieldRange MinAttackRange;
        public FieldRange MaxAttackRange;

        //То, в каком поле юнит находится
        public FieldRange FieldAffilation;

        public int ArmourValue;
        public ArmourType ArmourType;

        public int Cost;
        public Tier Tier;
    }
}
