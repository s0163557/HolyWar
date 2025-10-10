using System.Collections.Generic;
using HolyWar.Units;

public abstract class TargetFilter
{
    public abstract bool IsFriendly { get; }

    public abstract bool IsFindTarget(IEnumerable<BaseUnit> fieldUnits, out List<BaseUnit> targetUnits);
}
