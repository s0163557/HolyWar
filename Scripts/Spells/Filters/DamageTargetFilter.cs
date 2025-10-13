using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolyWar.Units;

namespace Assets.Scripts.Spells.Filters
{
    internal class DamageTargetFilter : TargetFilter
    {

        public override bool IsFindTarget(IEnumerable<BaseUnit> fieldUnits, out List<BaseUnit> targetUnits)
        {
            BaseUnit? minHpUnit = null;
            foreach (var unit in fieldUnits)
            {
                if (unit.CurrentHealth != unit.Stats.MaxHealth && unit.CurrentHealth < minHpUnit.CurrentHealth)
                {
                    minHpUnit = unit;
                }
            }

            if (minHpUnit == null)
            {
                targetUnits = new List<BaseUnit>();
                return false;
            }
            else
            {
                targetUnits = new List<BaseUnit> { minHpUnit };
                return true;
            }
        }
    }
}
