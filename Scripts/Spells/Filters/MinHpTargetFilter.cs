using HolyWar.Units;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Ищет одного юнита с самым маленьким количеством текущего хп
/// </summary>
[Serializable]
internal class MinHpTargetFilter : TargetFilter
{
    public override bool IsFindTarget(IEnumerable<BaseUnit> fieldUnits, out List<BaseUnit> targetUnits)
    {
        bool isFindUnit = false;
        BaseUnit minHpUnit = fieldUnits.FirstOrDefault();
        foreach (var unit in fieldUnits)
        {
            //Проверим, является ли рассматриваемый юнит продамаженным и затем посмотрим какое у него хп, и сравним с нашим 
            if (unit != null && (unit.States & BaseUnit.UnitState.Damaged) != 0 && unit.CurrentHealth <= minHpUnit.CurrentHealth)
            {
                minHpUnit = unit;
                isFindUnit = true;
            }
        }

        //Если мы по итогу никого не нашли, то возвращаем пустой список
        if (!isFindUnit)
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

