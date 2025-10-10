using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolyWar.Units;

/// <summary>
/// Ищет юнита для отхила с самым маленьким количеством текущего хп, пропускает фулл хпшных юнитов
/// </summary>
internal class HealTargetFilter : TargetFilter
{
    public override bool IsFriendly => true;

    public override bool IsFindTarget(IEnumerable<BaseUnit> fieldUnits, out List<BaseUnit> targetUnits)
    {
        BaseUnit? minHpUnit = null;
        foreach (var unit in fieldUnits)
        {
            if (unit.GetCurrentHealth() != unit.Stats.MaxHealth && unit.GetCurrentHealth() < minHpUnit.GetCurrentHealth())
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

