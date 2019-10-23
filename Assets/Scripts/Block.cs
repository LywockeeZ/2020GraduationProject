﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : State
{
    private float height = 0.1f;
    private GameObject model;

    private bool canWalk = true;
    private bool canBeFire = false;

    public Block(BaseUnit owner) : base(owner)
    {
        stateType = Enum.ENUM_State.Block;
        _stateName = "Block";
        OnStateBegin();
    }

    public override void OnStateBegin()
    {
        Owner.SetCanWalk(canWalk);
        Owner.SetCanBeFire(canBeFire);
        Owner.SetUpperType(Enum.ENUM_UpperUnitType.NULL);
        SetBlockModel();
    }

    public override void OnStateHandle()
    {
        
    }

    public override void OnStateEnd()
    {
        model.SetActive(false);
    }

    private void SetBlockModel()
    {
        GameObject BlockModel = Resources.Load("Prefabs/Block") as GameObject;
        model = GameObject.Instantiate(BlockModel, GetTargetPos(Owner.Model.transform.position, height), Quaternion.identity);
    }


}
