using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : IState
{
    //模型与模型生成高度增量
    private GameObject model;
    private float height = 0f;

    //该状态私有属性
    private bool canWalk = true;
    private bool canBeFire = false;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.False;


    public Fire(BaseUnit owner) : base(owner)
    {
        stateType = ENUM_State.Fire;
        _stateName = "Fire";
        beFiredType = _beFiredType;
        OnStateBegin();
    }



    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
        //关卡记录火焰信息
        Owner.GetStage().fireUnits.Add(Owner);

        //对关卡回合更新事件进行注册
        RegisterEvent();
        SetFireModel();
    }



    private EventListenerDelegate OnFireUpdate;
    private void RegisterEvent()
    {

        Game.Instance.RegisterEvent(ENUM_GameEvent.FireUpdate,
        OnFireUpdate = (Message evt) =>
        {
            OnStateHandle();
        });

    }




    public override void OnStateHandle()
    {
        Firing(Owner.Up);
        Firing(Owner.Down);
        Firing(Owner.Left);
        Firing(Owner.Right);
        Owner.GetStage().isFireUpdateEnd = true;
    }




    public override void OnStateEnd()
    {
        Owner.GetStage().fireUnits.Remove(Owner);

        Game.Instance.DetachEvent(ENUM_GameEvent.FireUpdate, OnFireUpdate);

        GameObject.Destroy(model);
    }



    //加载火焰模型
    private void SetFireModel()
    {
        GameObject fireModel = Resources.Load("Prefabs/Fire") as GameObject;
        model = GameObject.Instantiate(fireModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
        model.transform.SetParent(GameObject.Find("Units").transform);
    }





    //将目标单元设置成Fire状态
    private void Firing(BaseUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.CanBeFire)
        {
            if (targetUnit.UpperGameObject != null && targetUnit.UpperUnit.Type != ENUM_UpperUnit.Player)
            {
                //判断上层单元是否为可燃类型
                if (targetUnit.UpperUnit.BeFiredType == ENUM_UpperUnitBeFiredType.BeFire)
                {
                    targetUnit.UpperGameObject.GetComponent<ICanBeFiredUnit>().HandleByFire();
                }
            }
            else
            {
                if (targetUnit.State.beFiredType == ENUM_StateBeFiredType.BeHandle)
                {
                    targetUnit.StateRequest();
                }
                else
                if (targetUnit.State.beFiredType == ENUM_StateBeFiredType.BeFire)
                {
                    targetUnit.StateEnd();
                    if (targetUnit.UpperUnit.Type != ENUM_UpperUnit.Player)
                    {
                        targetUnit.SetState(new Fire(targetUnit));
                    }
                }
            }

        }
    }
}
