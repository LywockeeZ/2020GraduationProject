using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class Oil : IState
{

    private bool canWalk = true;
    private bool canBeFire = true;

    private float height = 0f;
    private GameObject model;


    public Oil(BaseUnit owner) : base(owner)
    {
        stateType = ENUM_State.Oil;
        _stateName = "Oil";
        OnStateBegin();
    }

    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
        Owner.GetStage().oilUnits.Add(Owner);

        SetOilModel();
    }

    public override void OnStateHandle()
    {
        //先将状态转换
        OnStateEnd();
        if (Owner.UpperType == ENUM_UpperUnitType.Movable)
        {
            Owner.SetState(new Block(Owner));

        }
        else
        if (Owner.UpperType == ENUM_UpperUnitType.Fixed)
        {
            Owner.UpperGameObject.GetComponent<IFixedUnit>().HandleByFire();
        }
        else Owner.SetState(new Fire(Owner));
        Owner.GetStage().isOilUpdateEnd = false;

        //如果周围有油就执行其行为
        GameManager.Instance.StartCoroutine(FireAround());

    }


    public override void OnStateEnd()
    {
        Owner.GetStage().oilUnits.Remove(Owner);
        GameObject.Destroy(model);
    }


    private void SetOilModel()
    {
        GameObject oilModel = Resources.Load("Prefabs/Oil") as GameObject;
        model = GameObject.Instantiate(oilModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
        model.transform.SetParent(GameObject.Find("Units").transform);
    }


    IEnumerator FireAround()
    {
        yield return new WaitForSeconds(0.5f);
        if (((Owner.Up    != null) && (Owner.Up.myState.stateType    == ENUM_State.Oil)) ||
            ((Owner.Down  != null) && (Owner.Down.myState.stateType  == ENUM_State.Oil)) ||
            ((Owner.Left  != null) && (Owner.Left.myState.stateType  == ENUM_State.Oil)) ||
            ((Owner.Right != null) && (Owner.Right.myState.stateType == ENUM_State.Oil)))
        {
            ChangeOilToFireState(Owner.Up);
            ChangeOilToFireState(Owner.Down);
            ChangeOilToFireState(Owner.Left);
            ChangeOilToFireState(Owner.Right);
        }
        else Owner.GetStage().isOilUpdateEnd = true;

    }


    //将状态为Oil的单元设置成Fire状态
    private void ChangeOilToFireState(BaseUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.myState.stateType == ENUM_State.Oil)
        {
            targetUnit.myState.OnStateHandle();
        }
        //else
        //if (targetUnit.UpperType == Enum.ENUM_UpperUnitType.Movable)
        //{
        //    targetUnit.myState.OnStateEnd();
        //    targetUnit.SetState(new Ground(targetUnit));
        //}
        
    }



}
