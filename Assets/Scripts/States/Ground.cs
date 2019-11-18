using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : IState
{
    private bool canWalk = true;
    private bool canBeFire = true;

    public Ground(BaseUnit owner) : base(owner)
    {
        stateType = ENUM_State.Ground;
        _stateName = "Ground";
        OnStateBegin();
    }

    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
        Owner.SetUpperType(ENUM_UpperUnitType.NULL);
    }

    public override void OnStateHandle()
    {

    }

    public override void OnStateEnd()
    {

    }
}
