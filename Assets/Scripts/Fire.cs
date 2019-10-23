using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : State
{
    private float height = 0.3f;
    private GameObject model;

    private bool canWalk = true;
    private bool canBeFire = false;

    public Fire(BaseUnit owner) : base(owner)
    {
        stateType = Enum.ENUM_State.Fire;
        _stateName = "Fire";
        OnStateBegin();
    }

    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
        Owner.SetUpperType(Enum.ENUM_UpperUnitType.NULL);
        SetFireModel();
        Owner.GetStage().fireUnits.Add(Owner);
    }

    public override void OnStateHandle()
    {
        ChangeToFireState(Owner.Up);
        ChangeToFireState(Owner.Down);
        ChangeToFireState(Owner.Left);
        ChangeToFireState(Owner.Right);
    }

    public override void OnStateEnd()
    {
        Owner.GetStage().fireUnits.Remove(Owner);
        model.SetActive(false);
    }

    private void SetFireModel()
    {
        GameObject fireModel = Resources.Load("Prefabs/Fire") as GameObject;
        model = GameObject.Instantiate(fireModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
    }

    //将目标单元设置成Fire状态
    private void ChangeToFireState(BaseUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.CanBeFire )
        {
            targetUnit.myState.OnStateEnd();
            targetUnit.SetState(new Fire(targetUnit));
        }
    }
}
