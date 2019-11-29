using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : IState
{
    private bool canWalk = true;
    private bool canBeFire = true;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.BeFire;

    public Ground(BaseUnit owner) : base(owner)
    {
        stateType = ENUM_State.Ground;
        _stateName = "Ground";
        beFiredType = _beFiredType;
        OnStateBegin();
    }

    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
    }

    public override void OnStateHandle()
    {

    }

    public override void OnStateEnd()
    {

    }
}
