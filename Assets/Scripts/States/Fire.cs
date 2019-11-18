﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : IState
{
    private bool canWalk = true;
    private bool canBeFire = false;

    private float height = 0f;
    private GameObject model;


    public Fire(BaseUnit owner) : base(owner)
    {
        stateType = ENUM_State.Fire;
        _stateName = "Fire";
        OnStateBegin();
    }



    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
        Owner.SetUpperType(ENUM_UpperUnitType.NULL);
        //关卡记录火焰信息
        Owner.GetStage().fireUnits.Add(Owner);
        //对关卡回合更新事件进行注册
        RegisterEvent();

        SetFireModel();
    }




    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.FireUpdate,
        delegate (Message evt)
        {
            OnStateHandle();
        });

    }




    public override void OnStateHandle()
    {
        ChangeToFireState(Owner.Up);
        ChangeToFireState(Owner.Down);
        ChangeToFireState(Owner.Left);
        ChangeToFireState(Owner.Right);
        Owner.GetStage().isFireUpdateEnd = true;
    }




    public override void OnStateEnd()
    {
        Owner.GetStage().fireUnits.Remove(Owner);

        Game.Instance.DetchEvent(ENUM_GameEvent.FireUpdate,
        delegate (Message evt)
        {
            OnStateHandle();
        });

        GameObject.Destroy(model);
    }




    private void SetFireModel()
    {
        GameObject fireModel = Resources.Load("Prefabs/Fire") as GameObject;
        model = GameObject.Instantiate(fireModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
    }





    //将目标单元设置成Fire状态
    private void ChangeToFireState(BaseUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.CanBeFire)
        {
            if (targetUnit.UpperGameObject != null)
            {
                if (targetUnit.UpperType == ENUM_UpperUnitType.Fixed)
                {
                    targetUnit.UpperGameObject.GetComponent<IFixedUnit>().HandleByFire();
                }
            }
            else
            {
                if (targetUnit.myState.stateType == ENUM_State.Oil)
                {
                    targetUnit.myState.OnStateHandle();
                }
                else
                {
                    targetUnit.myState.OnStateEnd();
                    targetUnit.SetState(new Fire(targetUnit));
                }
            }

        }
    }
}
