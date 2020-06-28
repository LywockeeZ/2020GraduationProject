using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : IState
{
    #region 私有属性
    //模型生成高度增量
    private float _height = 0f;
    private bool _canWalk = true;
    private new bool _canBeFire = true;
    private ENUM_StateBeFiredType _beFiredType = ENUM_StateBeFiredType.BeHandle;

    private int beFiredCount = 2;   //水能承受火的次数
    private bool isUpdating = false;//切换为雾的标签
    private GameObject waterEffect;
    private GameObject waterSmokeEffect;
    private MeshRenderer meshRenderer;
    private EventListenerDelegate OnSetWaterTexture;
    private bool _isSetTexture = false;
    #endregion

    public Water(BaseUnit owner, bool isSetTexture = true) : base(owner)
    {
        StateType = ENUM_State.Water;
        _stateName = "Water";
        BeFiredType = _beFiredType;
        _isSetTexture = isSetTexture;
        OnStateBegin();

        if (isSetTexture)
        {
            SetWaterTexture();
            Game.Instance.NotifyEvent(ENUM_GameEvent.SetWaterTexture);
        }

        Game.Instance.RegisterEvent(ENUM_GameEvent.SetWaterTexture, OnSetWaterTexture = (Message evt) =>
        {
            SetWaterTexture();
        });

    }


    public override void OnStateBegin()
    {
        Owner.SetCanWalk(_canWalk);
        Owner.SetCanBeFire(_canBeFire);

        RegisterEvent();
        SetWaterModel();
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
                    Owner.SetState(new Block(Owner));
                }
                else
                {
                    Owner.SetState(new Fire(Owner));
                }
            }
            else
            {
                GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
                SetWaterFogModel();             
            }

        }
    }


    public override void OnStateEnd()
    {
        DetachEvnt();
        Game.Instance.NotifyEvent(ENUM_GameEvent.SetWaterTexture);
        if (waterSmokeEffect != null)
        {
            Model.GetComponent<AudioSource>().PlayOneShot(Model.GetComponent<MyAudios>().audioClips[2]);
        }
        Model.transform.GetChild(0).GetComponent<MeshRenderer>().material.DOFade(0, 0.5f).OnComplete(() => {
            GameObject.Destroy(waterEffect);
            GameObject.Destroy(waterSmokeEffect);
            GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);
        });
    }


    #region 事件的注册与销毁
    private EventListenerDelegate OnRoundUpdateEnd;
    private void RegisterEvent()
    {

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundUpdateEnd,
        OnRoundUpdateEnd = (Message evt) =>
        {
            isUpdating = false;
        });

    }

    private void DetachEvnt()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundUpdateEnd, OnRoundUpdateEnd);
        Game.Instance.DetachEvent(ENUM_GameEvent.SetWaterTexture, OnSetWaterTexture);
    }
    #endregion


    /// <summary>
    /// 设置水的模型
    /// </summary>
    private void SetWaterModel()
    {
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("Water",
            GetTargetPos(Owner.Model.transform.position, _height));
        Model.transform.SetParent(Owner.Model.transform);
        meshRenderer = Model.transform.GetChild(0).GetComponent<MeshRenderer>();
        meshRenderer.GetComponent<MeshRenderer>().material.DOFade(1, 0.5f).From(0);

        if (_isSetTexture)
        {
            waterEffect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/WaterDrop", Owner.transform.position);
            int n = Random.Range(0, 2);
            Model.GetComponent<AudioSource>().PlayOneShot(Model.GetComponent<MyAudios>().audioClips[n]);
        }

    }


    /// <summary>
    /// 设置雾的模型
    /// </summary>
    private void SetWaterFogModel()
    {
        waterSmokeEffect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/WaterSmoke", Owner.transform.position);
        waterSmokeEffect.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        Model = GameFactory.GetAssetFactory().InstantiateGameObject("WaterFog",
            GetTargetPos(Owner.Model.transform.position, _height));
        Model.transform.SetParent(Owner.Model.transform);
        meshRenderer = Model.transform.GetChild(0).GetComponent<MeshRenderer>();
        meshRenderer.GetComponent<MeshRenderer>().material.DOFade(1, 0.5f).From(0);
        SetWaterTexture();
        Model.GetComponent<AudioSource>().PlayOneShot(Model.GetComponent<MyAudios>().audioClips[2]);
    }


    /// <summary>
    /// 检测周围是否有火焰
    /// </summary>
    /// <returns></returns>
    private bool IsHavingFireAround()
    {
        bool isExist = false;
        if ((Owner.Up    != null) && (Owner.Up.State.StateType    == ENUM_State.Fire) ||
            (Owner.Down  != null) && (Owner.Down.State.StateType  == ENUM_State.Fire) ||
            (Owner.Left  != null) && (Owner.Left.State.StateType  == ENUM_State.Fire) ||
            (Owner.Right != null) && (Owner.Right.State.StateType == ENUM_State.Fire))
        {
            isExist = true;
        }

        return isExist;
    }

    private void SetWaterTexture()
    {
        meshRenderer.material.mainTextureOffset = GetTextureOffset(GetAroundState());
    }

    private int GetAroundState()
    {
        int weight = 0;
        if (Owner.Up != null && Owner.Up.State.StateType == ENUM_State.Water)
            weight += 1;
        if (Owner.Right != null && Owner.Right.State.StateType == ENUM_State.Water)
            weight += 4;
        if (Owner.Down != null && Owner.Down.State.StateType == ENUM_State.Water)
            weight += 9;
        if (Owner.Left != null && Owner.Left.State.StateType == ENUM_State.Water)
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
