using System;
using System.Collections.Generic;
using HolyWar.Units;

[Serializable]
public abstract class TargetFilter
{
    public abstract bool IsFindTarget(IEnumerable<BaseUnit> fieldUnits, out List<BaseUnit> targetUnits);
}
