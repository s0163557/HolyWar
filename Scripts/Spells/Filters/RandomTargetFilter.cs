using HolyWar.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

/// <summary>
/// Ищет одного случайного юнита
/// </summary>
[Serializable]
internal class RandomTargetFilter : TargetFilter
{

    public override bool IsFindTarget(IEnumerable<BaseUnit> fieldUnits, out List<BaseUnit> targetUnits)
    {
        if (fieldUnits.Any())
        {
            var unitPosition = UnityEngine.Random.Range(0, fieldUnits.Count() - 1);
            targetUnits = new List<BaseUnit>() { fieldUnits.ElementAt(unitPosition) };
            return true;
        }
        else
        {
            targetUnits = new List<BaseUnit>();
            return false;
        }
    }
}

