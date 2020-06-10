using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Fungus;

public class Block : IState
{

    #region 私有属性
    //模型生成高度增量
    private float _height = 0f;
    private bool _canWalk = true;
    private new bool _canBeFire = false;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.False;
    private GameObject blockEffect = null;
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
        Model.transform.GetChild(0).GetComponent<MeshRenderer>().material.DOFade(0, 0.5f).OnComplete(() => {
            GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
            GameObject.Destroy(blockEffect);
            Owner = null;
        });

    }


    /// <summary>
    /// 加载模型，设置下层单元为父物体
    /// </summary>
    private void SetBlockModel()
    {
        blockEffect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/DustExplosion", Owner.transform.position);
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("Block",
                    GetTargetPos(Owner.Model.transform.position, _height));
        Model.transform.SetParent(Owner.Model.transform);
        Model.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2((Model.transform.position.x % 3) * 0.333f, (Model.transform.position.z % 3) * 0.333f);
        Model.transform.GetChild(0).GetComponent<MeshRenderer>().material.DOFade(1, 0.5f).From(0);
    }


}
