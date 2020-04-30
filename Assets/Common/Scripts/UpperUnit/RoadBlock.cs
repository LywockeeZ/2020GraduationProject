using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlock : MonoBehaviour, IUpperUnit, IFixedUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float    Height    { get { return _heigth   ; } set { _heigth    = value; } }
    public bool     CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }


    #region 私有属性
    private ENUM_UpperUnit            Type        = ENUM_UpperUnit.RoadBlock;         //放置单元的类型
    private ENUM_UpperUnitControlType ControlType = ENUM_UpperUnitControlType.Fixed;  //放置单元的操控类型
    private ENUM_UpperUnitBeFiredType BeFiredType = ENUM_UpperUnitBeFiredType.NULL;   

    //该单元的私有属性
    private BaseUnit _currentOn;
    private float    _heigth = 0f;
    private bool     _canBeFire = false;
    #endregion


    public void Init()
    {
        _currentOn.StateEnd();
        _currentOn.SetState(new Ground(_currentOn));
        _currentOn.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
        _currentOn.SetUpperGameObject(gameObject);
        _currentOn.SetCanBeFire(_canBeFire);

        transform.position = SetTargetPos(transform.position);
    }


    public void End()
    {
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(this.gameObject);
        _currentOn.UpperUnit.InitOrReset();
        CurrentOn.UpperGameObject = null;
        Destroy(this);
    }


    private void Start()
    {
        Init();
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


    public void Handle()
    {

    }


    public void HandleByFire()
    {

    }
}
