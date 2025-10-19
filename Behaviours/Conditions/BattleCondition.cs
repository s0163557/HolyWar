using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Test", story: "No battle is going [bool]", category: "Conditions", id: "bad0be26a5d837a5a7096385ddb9b2b8")]
public partial class BattleCondition : Condition
{
    [SerializeReference] public BlackboardVariable<bool> Bool;

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
