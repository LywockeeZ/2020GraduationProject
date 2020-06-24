using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using DG.Tweening;

public class Oil : IState
{

    #region 私有属性
    //该状态模型增量
    private float _height = 0f;
    private bool _canWalk = true;
    private new bool _canBeFire = true;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.BeHandle;
    private GameObject oilEffect;
    private GameObject oilStayEffect;
    private MeshRenderer meshRenderer;
    private EventListenerDelegate OnSetOilTexture;
    private bool _isSetTexture = false;
    #endregion

    public Oil(BaseUnit owner, bool isSetTexture = true) : base(owner)
    {
        StateType = ENUM_State.Oil;
        _stateName = "Oil";
        BeFiredType = _beFiredType;
        _isSetTexture = isSetTexture;
        OnStateBegin();

        if (isSetTexture)
        {
            SetOilTexture();
            Game.Instance.NotifyEvent(ENUM_GameEvent.SetOilTexture);
        }

        Game.Instance.RegisterEvent(ENUM_GameEvent.SetOilTexture, OnSetOilTexture = (Message evt) =>
        {
            SetOilTexture();
        });
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
        Model.transform.GetChild(0).GetComponent<MeshRenderer>().material.DOFade(0, 0.5f).OnComplete(() => {
            Owner.GetStage().oilUnits.Remove(Owner);
            GameObject.Destroy(oilEffect);
            if (oilStayEffect != null)
            {
                GameObject.Destroy(oilStayEffect);
            }
            GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
        });
        Owner.State.StateType = ENUM_State.Ground;
        Game.Instance.DetachEvent(ENUM_GameEvent.SetOilTexture, OnSetOilTexture);
        Game.Instance.NotifyEvent(ENUM_GameEvent.SetOilTexture);
    }


    /// <summary>
    /// 设置油的模型
    /// </summary>
    private void SetOilModel()
    {
        if (_isSetTexture)
        {
            oilEffect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/OilDrop", Owner.transform.position);
        }
        int numb = Random.Range(0, 4);
        if (numb < 1)
        {
            oilStayEffect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/OilStayEffect", Owner.transform.position);
            oilStayEffect.transform.forward = Vector3.up;

        }
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("Oil",
            GetTargetPos(Owner.Model.transform.position, _height));
        Model.transform.SetParent(Owner.Model.transform);
        meshRenderer = Model.transform.GetChild(0).GetComponent<MeshRenderer>();
        meshRenderer.material.DOFade(1, 0.5f).From(0);
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
            if (targetUnit.UpperGameObject != null && targetUnit.UpperUnit.BeFiredType == ENUM_UpperUnitBeFiredType.BeFire)
            {
                targetUnit.UpperGameObject.GetComponent<ICanBeFiredUnit>().HandleByFire();
            }
            else
                targetUnit.StateRequest();
        }
        
    }

    private void SetOilTexture()
    {
        meshRenderer.material.mainTextureOffset = GetTextureOffset(GetAroundState());
    }

    private int GetAroundState()
    {
        int weight = 0;
        if (Owner.Up != null && Owner.Up.State.StateType == ENUM_State.Oil)
            weight += 1;
        if (Owner.Right != null && Owner.Right.State.StateType == ENUM_State.Oil)
            weight += 4;
        if (Owner.Down != null && Owner.Down.State.StateType == ENUM_State.Oil)
            weight += 9;
        if (Owner.Left != null && Owner.Left.State.StateType == ENUM_State.Oil)
            weight += 16;

        return weight;
    }

    private Vector2 GetTextureOffset(int weight)
    {
        Vector2 textureOffset = Vector2.zero;
        switch (weight)
        {
            case 0:
                textureOffset = new Vector2(0, 0);
                break;
            case 1:
                textureOffset = new Vector2(0.6666f, 0.3334f);
                break;
            case 4:
                textureOffset = new Vector2(0, 0.1667f);
                break;
            case 5:
                textureOffset = new Vector2(0, 0.501f);
                break;
            case 9:
                textureOffset = new Vector2(0, 0.3334f);
                break;
            case 10:
                textureOffset = new Vector2(0.3333f, 0.3334f);
                break;
            case 13:
                textureOffset = new Vector2(0, 0.8335f);
                break;
            case 14:
                textureOffset = new Vector2(0, 0.6668f);
                break;
            case 16:
                textureOffset = new Vector2(0.6666f, 0.1667f);
                break;
            case 17:
                textureOffset = new Vector2(0.6666f, 0.501f);
                break;
            case 20:
                textureOffset = new Vector2(0.3333f, 0.1667f);
                break;
            case 21:
                textureOffset = new Vector2(0.3333f, 0.501f);
                break;
            case 25:
                textureOffset = new Vector2(0.6666f, 0.8335f);
                break;
            case 26:
                textureOffset = new Vector2(0.6666f, 0.6668f);
                break;
            case 29:
                textureOffset = new Vector2(0.3333f, 0.8335f);
                break;
            case 30:
                textureOffset = new Vector2(0.3333f, 0.6668f);
                break;
            default:
                Debug.LogError("未找到该权重:" + weight);
                textureOffset = new Vector2(0, 0);
                break;
        }
        return textureOffset;
    }

}
