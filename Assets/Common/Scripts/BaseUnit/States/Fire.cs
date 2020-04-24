using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fire : IState
{

    #region 私有字段
    //模型生成高度增量
    private float height = 0f;
    private bool canWalk = true;
    private bool canBeFire = false;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.False;
    private FireController fireController;
    #endregion

    public Fire(BaseUnit owner) : base(owner)
    {
        StateType = ENUM_State.Fire;
        _stateName = "Fire";
        BeFiredType = _beFiredType;
        OnStateBegin();
    }


    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
        Owner.GetStage().fireUnits.Add(Owner);  //关卡记录火焰信息

        SetFireModel();
        RegisterEvent();                       //对关卡回合更新事件进行注册

        if (!Game.Instance.isTest)
        {
            FreeCamController.Instance.AddTarget(Model.transform, 2, 0);
        }
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
        if (!Game.Instance.isTest)
        {
            FreeCamController.Instance.RemoveTarget(Model.transform);
        }

        Owner.GetStage().fireUnits.Remove(Owner);

        DetachEvent();

        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
    }


    public override void OnStateEnd(Action callBack)
    {
        fireController.FireDisappear();
        void action()
        {
            OnStateEnd();
            callBack?.Invoke();
        }
        CoroutineManager.StartCoroutineTask(fireController.IsFireEnd, action, 0f);
    }


    #region 事件的注册与销毁
    private EventListenerDelegate OnFireUpdate;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.FireUpdate, OnFireUpdate = (Message evt) => {
            OnStateHandle();
        });
    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.FireUpdate, OnFireUpdate);
    }
    #endregion


    /// <summary>
    /// 加载火焰模型
    /// </summary>
    private void SetFireModel()
    {
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("Fire",
            GetTargetPos(Owner.Model.transform.position, height));
        Model.transform.SetParent(Owner.Model.transform);
        fireController = Model.GetComponent<FireController>();
    }


    /// <summary>
    /// 将目标单元设置成Fire状态
    /// </summary>
    /// <param name="targetUnit"></param>
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
                if (targetUnit.State.BeFiredType == ENUM_StateBeFiredType.BeHandle)
                {
                    targetUnit.StateRequest();
                }
                else
                if (targetUnit.State.BeFiredType == ENUM_StateBeFiredType.BeFire)
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
