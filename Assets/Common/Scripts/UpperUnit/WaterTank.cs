using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTank : MonoBehaviour, IUpperUnit, IFixedUnit, ICanBeFiredUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float    Height    { get { return _heigth   ; } set { _heigth    = value; } }
    public bool     CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }

    public Animator animator;


    #region 私有属性
    private ENUM_UpperUnit Type = ENUM_UpperUnit.WaterTank;                            //放置单元的类型
    private ENUM_UpperUnitControlType ControlType = ENUM_UpperUnitControlType.Fixed;   //放置单元的操控类型
    private ENUM_UpperUnitBeFiredType BeFiredType = ENUM_UpperUnitBeFiredType.BeFire;

    private BaseUnit _currentOn;
    private float    _heigth    = 0f;
    private bool     _canBeFire = true;

    private BrokeEvent brokeEvent;
    #endregion


    private void Start()
    {
        Init();
    }


    public void Init()
    {
        //初始化状态
        _currentOn.SetState(new Ground(_currentOn));
        _currentOn.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
        _currentOn.SetUpperGameObject(gameObject);
        _currentOn.SetCanBeFire(_canBeFire);

        transform.position = SetTargetPos(transform.position);

        animator = transform.GetChild(0).GetComponent<Animator>();
        brokeEvent = transform.GetChild(0).GetComponent<BrokeEvent>();
    }


    public void End()
    {
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(this.gameObject);
        _currentOn.UpperUnit.InitOrReset();
        CurrentOn.UpperGameObject = null;
        Destroy(this);
    }


    public void Handle()
    {
        Game.Instance.CostAP(1, 0);

        if (_currentOn.State.StateType != ENUM_State.Oil)
        {
            _currentOn.StateEnd();
            _currentOn.SetState(new Water(_currentOn));
        }
        SetAroundToWater();
        if (animator != null) animator.SetTrigger("Break");
        brokeEvent.Broken();
        End();
    }


    public void HandleByFire()
    {
        _currentOn.StateEnd();
        _currentOn.SetState(new Water(_currentOn));
        SetAroundToWater();
        if (animator != null) animator.SetTrigger("Break");
        brokeEvent.Broken();
        End();
    }


    /// <summary>
    /// 将单元的高度转换为模型高度
    /// </summary>
    /// <param name="_targetPos"></param>
    /// <returns></returns>
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _targetPos.y + _heigth, _targetPos.z);
    }


    private void SetAroundToWater()
    {
        SetTargetToWater(_currentOn.Up);
        SetTargetToWater(_currentOn.Down);
        SetTargetToWater(_currentOn.Left);
        SetTargetToWater(_currentOn.Right);
    }


    private void SetTargetToWater(BaseUnit targetUnit)
    {
        if (targetUnit != null && 
            targetUnit.UpperUnit.Type == ENUM_UpperUnit.NULL &&
            (targetUnit.State.StateType == ENUM_State.Fire || targetUnit.State.StateType == ENUM_State.Ground))
        {
            targetUnit.StateEnd();
            targetUnit.SetState(new Water(targetUnit));
        }
    }
}
