using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : IState
{

    #region 私有属性
    //模型生成高度增量
    private float _height = 0f;
    private bool _canWalk = true;
    private new bool _canBeFire = true;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.BeFire;
    ////析构函数
    //~Ground()
    //{
    //    Debug.Log("GroundState已销毁");
    //}
    #endregion


    public Ground(BaseUnit owner) : base(owner)
    {
        StateType = ENUM_State.Ground;
        _stateName = "Ground";
        BeFiredType = _beFiredType;
        OnStateBegin();
    }


    public override void OnStateBegin()
    {
        Owner.SetCanWalk(_canWalk);
        Owner.SetCanBeFire(_canBeFire);

        SetGroundModel();
    }


    public override void OnStateHandle()
    {

    }


    public override void OnStateEnd()
    {
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
    }

    private void SetGroundModel()
    {
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("Ground",
                    GetTargetPos(Owner.Model.transform.position, _height));
        Model.transform.SetParent(Owner.Model.transform);

    }
}
