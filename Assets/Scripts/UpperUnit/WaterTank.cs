using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTank : MonoBehaviour, IUpperUnit, IFixedUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float Height { get { return _heigth; } set { _heigth = value; } }
    public bool CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }

    private BaseUnit _currentOn;
    private float _heigth = 0.3f;
    private bool _canBeFire = false;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        //初始化状态
        _currentOn.myState.OnStateEnd();
        _currentOn.SetState(new Ground(_currentOn));
        _currentOn.SetUpperType(Enum.ENUM_UpperUnitType.Fixed);
        _currentOn.SetUpperGameObject(gameObject);

        transform.position = SetTargetPos(transform.position);

    }


    public void Handle()
    {
        GameManager.Instance.ReducePoints(1, 0);
        if (_currentOn.myState.stateType != Enum.ENUM_State.Oil)
        {
            _currentOn.myState.OnStateEnd();
            _currentOn.SetState(new Water(_currentOn));
        }
        SetAroundToWater();
        End();
    }

    public void HandleByFire()
    {
        _currentOn.myState.OnStateEnd();
        _currentOn.SetState(new Water(_currentOn));
        SetAroundToWater();
        End();
    }

    public void End()
    {
        _currentOn.SetUpperType(Enum.ENUM_UpperUnitType.NULL);
        GameObject.Destroy(gameObject);
    }


    //将单元的高度转换为模型高度
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _heigth, _targetPos.z);
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
            targetUnit.UpperType == Enum.ENUM_UpperUnitType.NULL &&
            (targetUnit.myState.stateType == Enum.ENUM_State.Fire || targetUnit.myState.stateType == Enum.ENUM_State.Ground))
        {
            targetUnit.myState.OnStateEnd();
            targetUnit.SetState(new Water(targetUnit));
        }
    }
}
