using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Fungus;
using System;

public class Block : IState
{

    #region 私有属性
    //模型生成高度增量
    private float _height = 0f;
    private bool _canWalk = true;
    private new bool _canBeFire = false;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.False;
    private GameObject blockEffect = null;
    /// <summary>
    /// 用于设置阻燃带连接处
    /// </summary>
    private BaseUnit _currentUnit = null;
    private GameObject blockConnect = null;
    #endregion


    public Block(BaseUnit owner) : base(owner)
    {
        StateType = ENUM_State.Block;
        _stateName = "Block";
        BeFiredType = _beFiredType;
        OnStateBegin();
    }

    public Block(BaseUnit owner, BaseUnit currentUnit):base(owner)
    {
        _currentUnit = currentUnit;

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
        Model.transform.GetChild(0).GetComponent<MeshRenderer>().material.DOFade(0, 0.5f).OnComplete(() => {
            GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
            GameObject.Destroy(blockEffect);
            Owner = null;
            if (_currentUnit != null)
            {
                GameObject.Destroy(blockConnect);
            }
        });

    }


    /// <summary>
    /// 加载模型，设置下层单元为父物体
    /// </summary>
    private void SetBlockModel()
    {
        blockEffect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/BlockDrop", Owner.transform.position);
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("Block",
                    GetTargetPos(Owner.Model.transform.position, _height));
        Model.transform.SetParent(Owner.Model.transform);
        Model.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(UnityEngine.Random.Range(0, 4) % 4 * 0.333f, 0);
        Model.transform.GetChild(0).GetComponent<MeshRenderer>().material.DOFade(1, 0.5f).From(0);

        if (_currentUnit != null && _currentUnit.State.StateType == ENUM_State.Block)
        {
            blockConnect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Prefabs/Units/BlockConnect", (_currentUnit.transform.position + Owner.transform.position) / 2);
            blockConnect.transform.forward = Owner.transform.position - _currentUnit.transform.position;

        }
    }


}
