﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlock : MonoBehaviour, IUpperUnit, IFixedUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float Height { get { return _heigth; } set { _heigth = value; } }
    public bool CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }

    //该单元的私有属性
    private BaseUnit _currentOn;
    private float _heigth = 0f;
    private bool _canBeFire = false;

    private void Start()
    {
        Init();
    }


    public void Init()
    {
        _currentOn.myState.OnStateEnd();
        _currentOn.SetState(new Ground(_currentOn));
        _currentOn.SetUpperType(Enum.ENUM_UpperUnitType.Fixed);
        _currentOn.SetUpperGameObject(gameObject);
        _currentOn.SetCanBeFire(_canBeFire);

        transform.position = SetTargetPos(transform.position);
    }


    public void End()
    {

    }


    //将单元的高度转换为模型高度
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _heigth, _targetPos.z);
    }

    public void Handle()
    {

    }

    public void HandleByFire()
    {

    }
}
