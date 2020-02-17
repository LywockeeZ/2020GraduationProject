﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : IState
{
    #region 私有属性
    //模型生成高度增量
    private float height = 0f;
    private bool canWalk = true;
    private bool canBeFire = true;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.BeHandle;

    private int beFiredCount = 2;   //水能承受火的次数
    private bool isUpdating = false;//切换为雾的标签
    #endregion

    public Water(BaseUnit owner) : base(owner)
    {
        StateType = ENUM_State.Water;
        _stateName = "Water";
        BeFiredType = _beFiredType;
        OnStateBegin();
    }


    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);

        RegisterEvent();
        SetWaterModel();
    }


    public override void OnStateHandle()
    {
        if (IsHavingFireAround() && !isUpdating)
        {
            isUpdating = true;
            beFiredCount--;
            if (beFiredCount == 0)
            {
                if (Owner.UpperUnit.Type == ENUM_UpperUnit.Player)
                {
                    OnStateEnd();
                    Owner.SetState(new Block(Owner));
                }
                else
                {
                    OnStateEnd();
                    Owner.SetState(new Fire(Owner));
                }
            }
            else
            {
                GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
                SetWaterFogModel();
            }

        }
    }


    public override void OnStateEnd()
    {
        DetachEvnt();
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
    }


    #region 事件的注册与销毁
    private EventListenerDelegate OnRoundUpdateEnd;
    private void RegisterEvent()
    {

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundUpdateEnd,
        OnRoundUpdateEnd = (Message evt) =>
        {
            isUpdating = false;
        });

    }

    private void DetachEvnt()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundUpdateEnd, OnRoundUpdateEnd);
    }
    #endregion


    /// <summary>
    /// 设置水的模型
    /// </summary>
    private void SetWaterModel()
    {
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("Water",
            GetTargetPos(Owner.Model.transform.position, height));
        Model.transform.SetParent(Owner.Model.transform);
    }


    /// <summary>
    /// 设置雾的模型
    /// </summary>
    private void SetWaterFogModel()
    {
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("WaterFog",
            GetTargetPos(Owner.Model.transform.position, height));
        Model.transform.SetParent(Owner.Model.transform);
    }


    /// <summary>
    /// 检测周围是否有火焰
    /// </summary>
    /// <returns></returns>
    private bool IsHavingFireAround()
    {
        bool isExist = false;
        if ((Owner.Up    != null) && (Owner.Up.State.StateType    == ENUM_State.Fire) ||
            (Owner.Down  != null) && (Owner.Down.State.StateType  == ENUM_State.Fire) ||
            (Owner.Left  != null) && (Owner.Left.State.StateType  == ENUM_State.Fire) ||
            (Owner.Right != null) && (Owner.Right.State.StateType == ENUM_State.Fire))
        {
            isExist = true;
        }

        return isExist;
    }


}
