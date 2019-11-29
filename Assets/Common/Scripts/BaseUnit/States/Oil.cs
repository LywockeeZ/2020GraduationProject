using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class Oil : IState
{
    private GameObject model;
    private float height = 0f;

    private bool canWalk = true;
    private bool canBeFire = true;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.BeHandle;



    public Oil(BaseUnit owner) : base(owner)
    {
        stateType = ENUM_State.Oil;
        _stateName = "Oil";
        beFiredType = _beFiredType;
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

        //如果油上方站的是玩家，则改成阻燃带
        //如果是可被点燃的物体，则点燃
        if (Owner.UpperUnit.Type == ENUM_UpperUnit.NULL)
        {
            Owner.SetState(new Fire(Owner));
        }
        else
        {
            if (Owner.UpperUnit.Type == ENUM_UpperUnit.Player)
            {
                Owner.SetState(new Block(Owner));
            }
            else
            if (Owner.UpperUnit.BeFiredType == ENUM_UpperUnitBeFiredType.BeFire)
            {
                Owner.UpperGameObject.GetComponent<ICanBeFiredUnit>().HandleByFire();
            }
            else
            if (Owner.UpperUnit.BeFiredType != ENUM_UpperUnitBeFiredType.NULL)
            {
                Owner.SetState(new Fire(Owner));
            }

            //如果上层是不可燃物则结束蔓延
            Owner.GetStage().isOilUpdateEnd = true;
            return;
        }

        Owner.GetStage().isOilUpdateEnd = false;

        //如果周围有油就执行其行为
        CoroutineManager.StartCoroutineTask(FireAround, 0.5f);

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


    public void FireAround()
    {
        //这里用来判定油的扩散结束
        if (((Owner.Up    != null) && (Owner.Up.State.stateType    == ENUM_State.Oil)) ||
            ((Owner.Down  != null) && (Owner.Down.State.stateType  == ENUM_State.Oil)) ||
            ((Owner.Left  != null) && (Owner.Left.State.stateType  == ENUM_State.Oil)) ||
            ((Owner.Right != null) && (Owner.Right.State.stateType == ENUM_State.Oil)))
        {
            Firing(Owner.Up);
            Firing(Owner.Down);
            Firing(Owner.Left);
            Firing(Owner.Right);
        }
        else Owner.GetStage().isOilUpdateEnd = true;

    }


    //将状态为Oil的单元设置成Fire状态
    private void Firing(BaseUnit targetUnit)
    {

        if (targetUnit != null && targetUnit.State.stateType == ENUM_State.Oil)
        {
            targetUnit.StateRequest();
        }
        
    }



}
