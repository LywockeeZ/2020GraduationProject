using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : State
{

    private bool canWalk = true;
    private bool canBeFire = true;

    private float height = 0.01f;
    private GameObject model;


    public Oil(BaseUnit owner) : base(owner)
    {
        stateType = Enum.ENUM_State.Oil;
        _stateName = "Oil";
        OnStateBegin();
    }

    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
        Owner.SetUpperType(Enum.ENUM_UpperUnitType.NULL);

        SetOilModel();
    }

    public override void OnStateHandle()
    {
        //先将状态转换
        OnStateEnd();
        Owner.SetState(new Fire(Owner));

        //如果周围有油就执行其行为
        if ((Owner.Up    != null) && (Owner.Up.myState.stateType    == Enum.ENUM_State.Oil) ||
            (Owner.Down  != null) && (Owner.Down.myState.stateType  == Enum.ENUM_State.Oil) ||
            (Owner.Left  != null) && (Owner.Left.myState.stateType  == Enum.ENUM_State.Oil) ||
            (Owner.Right != null) && (Owner.Right.myState.stateType == Enum.ENUM_State.Oil))
        {
            FireAround();           
        }
    }


    public override void OnStateEnd()
    {
        GameObject.Destroy(model);
    }


    private void SetOilModel()
    {
        GameObject oilModel = Resources.Load("Prefabs/Oil") as GameObject;
        model = GameObject.Instantiate(oilModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
    }


    private void FireAround()
    {
        ChangeOilToFireState(Owner.Up);
        ChangeOilToFireState(Owner.Down);
        ChangeOilToFireState(Owner.Left);
        ChangeOilToFireState(Owner.Right);
    }


    //将状态为Oil的单元设置成Fire状态
    private void ChangeOilToFireState(BaseUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.myState.stateType == Enum.ENUM_State.Oil)
        {
            targetUnit.myState.OnStateHandle();
        }
        
    }

}
