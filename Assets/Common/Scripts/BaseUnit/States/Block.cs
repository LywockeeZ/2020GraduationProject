using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : IState
{

    #region 私有属性
    //模型生成高度增量
    private float _height = 0f;
    private bool _canWalk = true;
    private new bool _canBeFire = false;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.False;
    #endregion


    public Block(BaseUnit owner) : base(owner)
    {
        StateType = ENUM_State.Block;
        _stateName = "Block";
        BeFiredType = _beFiredType;
        OnStateBegin();
    }


    public override void OnStateBegin()
    {
        Owner.SetCanWalk(_canWalk);
        Owner.SetCanBeFire(_canBeFire);

        SetBlockModel();
    }


    public override void OnStateHandle()
    {
        
    }


    public override void OnStateEnd()
    {
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
        Owner = null;
    }


    /// <summary>
    /// 加载模型，设置下层单元为父物体
    /// </summary>
    private void SetBlockModel()
    {
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("Block",
                    GetTargetPos(Owner.Model.transform.position, _height));
        Model.transform.SetParent(Owner.Model.transform);
    }


}
