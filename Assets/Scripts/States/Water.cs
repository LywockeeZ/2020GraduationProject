using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : State
{
    private bool canWalk = true;
    private bool canBeFire = false;

    private float height = 0f;
    private GameObject model;
    //水能承受火的次数
    private int beFiredCount = 2;


    public Water(BaseUnit owner) : base(owner)
    {
        stateType = Enum.ENUM_State.Water;
        _stateName = "Water";
        OnStateBegin();
    }

    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
        Owner.GetStage().WaterUpdateEvent += OnStateHandle;

        SetWaterModel();
    }

    public override void OnStateHandle()
    {
        if (IsHavingFireAround())
        {
            beFiredCount--;
            if (beFiredCount == 0)
            {
                if (Owner.UpperType == Enum.ENUM_UpperUnitType.Movable)
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
        Owner.GetStage().WaterUpdateEvent -= OnStateHandle;
        GameObject.Destroy(model);
    }

    private void SetWaterModel()
    {
        GameObject waterModel = Resources.Load("Prefabs/Water") as GameObject;
        model = GameObject.Instantiate(waterModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
    }

    private void SetWaterFogModel()
    {
        GameObject waterFogModel = Resources.Load("Prefabs/WaterFog") as GameObject;
        model = GameObject.Instantiate(waterFogModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
    }

    public bool IsHavingFireAround()
    {
        bool isExist = false;
        if ((Owner.Up != null) && (Owner.Up.myState.stateType == Enum.ENUM_State.Fire) ||
            (Owner.Down != null) && (Owner.Down.myState.stateType == Enum.ENUM_State.Fire) ||
            (Owner.Left != null) && (Owner.Left.myState.stateType == Enum.ENUM_State.Fire) ||
            (Owner.Right != null) && (Owner.Right.myState.stateType == Enum.ENUM_State.Fire))
        {
            isExist = true;
        }

        return isExist;
    }


}
