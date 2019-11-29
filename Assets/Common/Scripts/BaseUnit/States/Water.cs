using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : IState
{
    //水能承受火的次数
    private int beFiredCount = 2;

    //模型与模型生成高度增量
    private GameObject model;
    private float height = 0f;

    //该状态私有属性
    private bool canWalk = true;
    private bool canBeFire = true;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.BeHandle;

    private bool isUpdating = false;


    public Water(BaseUnit owner) : base(owner)
    {
        stateType = ENUM_State.Water;
        _stateName = "Water";
        beFiredType = _beFiredType;
        OnStateBegin();
    }

    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);

        RegisterEvent();
        SetWaterModel();
    }


    private EventListenerDelegate OnRoundUpdateEnd;
    private void RegisterEvent()
    {

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundUpdateEnd,
        OnRoundUpdateEnd = (Message evt) =>
        {
            isUpdating = false;
        });

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
                GameObject.Destroy(model);
                SetWaterFogModel();
            }

        }
    }

    public override void OnStateEnd()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundUpdateEnd, OnRoundUpdateEnd);

        GameObject.Destroy(model);
    }

    private void SetWaterModel()
    {
        GameObject waterModel = Resources.Load("Prefabs/Water") as GameObject;
        model = GameObject.Instantiate(waterModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
        model.transform.SetParent(GameObject.Find("Units").transform);
    }

    private void SetWaterFogModel()
    {
        GameObject waterFogModel = Resources.Load("Prefabs/WaterFog") as GameObject;
        model = GameObject.Instantiate(waterFogModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
        model.transform.SetParent(GameObject.Find("Units").transform);
    }

    public bool IsHavingFireAround()
    {
        bool isExist = false;
        if ((Owner.Up    != null) && (Owner.Up.State.stateType    == ENUM_State.Fire) ||
            (Owner.Down  != null) && (Owner.Down.State.stateType  == ENUM_State.Fire) ||
            (Owner.Left  != null) && (Owner.Left.State.stateType  == ENUM_State.Fire) ||
            (Owner.Right != null) && (Owner.Right.State.stateType == ENUM_State.Fire))
        {
            isExist = true;
        }

        return isExist;
    }


}
