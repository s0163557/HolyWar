using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "NoActiveBattle", story: "not [battle]", category: "Conditions", id: "4cedf473c6fa0de64a01317dbc29664c")]
public partial class NoActiveBattleCondition : Condition
{
    [SerializeReference] public BlackboardVariable<bool> Battle;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
