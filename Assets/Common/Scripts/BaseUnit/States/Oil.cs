using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class Oil : IState
{

    #region 私有属性
    //该状态模型增量
    private float _height = 0f;
    private bool _canWalk = true;
    private new bool _canBeFire = true;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.BeHandle;
    #endregion

    public Oil(BaseUnit owner) : base(owner)
    {
        StateType = ENUM_State.Oil;
        _stateName = "Oil";
        BeFiredType = _beFiredType;
        OnStateBegin();
    }


    public override void OnStateBegin()
    {
        Owner.SetCanWalk(_canWalk);
        Owner.SetCanBeFire(_canBeFire);
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
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
    }


    /// <summary>
    /// 设置油的模型
    /// </summary>
    private void SetOilModel()
    {
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("Oil",
            GetTargetPos(Owner.Model.transform.position, _height));
        Model.transform.SetParent(Owner.Model.transform);
        Model.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2((Model.transform.position.x % 3) * 0.333f, (Model.transform.position.z % 3) * 0.333f);
    }


    /// <summary>
    /// 点燃四周单元
    /// </summary>
    private void FireAround()
    {
        //这里用来判定油的扩散结束
        if (((Owner.Up    != null) && (Owner.Up.State.StateType    == ENUM_State.Oil)) ||
            ((Owner.Down  != null) && (Owner.Down.State.StateType  == ENUM_State.Oil)) ||
            ((Owner.Left  != null) && (Owner.Left.State.StateType  == ENUM_State.Oil)) ||
            ((Owner.Right != null) && (Owner.Right.State.StateType == ENUM_State.Oil)))
        {
            Firing(Owner.Up);
            Firing(Owner.Down);
            Firing(Owner.Left);
            Firing(Owner.Right);
        }
        else Owner.GetStage().isOilUpdateEnd = true;

    }


    /// <summary>
    /// 将状态为Oil的单元设置成Fire状态
    /// </summary>
    /// <param name="targetUnit"></param>
    private void Firing(BaseUnit targetUnit)
    {

        if (targetUnit != null && targetUnit.State.StateType == ENUM_State.Oil)
        {
            targetUnit.StateRequest();
        }
        
    }

}
