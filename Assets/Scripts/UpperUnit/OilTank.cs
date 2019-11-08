using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilTank : MonoBehaviour, IUpperUnit, IFixedUnit
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
        _currentOn.myState.OnStateEnd();
        _currentOn.SetState(new Oil(_currentOn));
        SetAroundToOil();
        End();
    }

    public void HandleByFire()
    {
        _currentOn.SetState(new Fire(_currentOn));

    }

    public void End()
    {
        GameObject.Destroy(gameObject);
    }


    //将单元的高度转换为模型高度
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _heigth, _targetPos.z);
    }

    private void SetAroundToOil()
    {
        SetTargetToOil(_currentOn.Up);
        SetTargetToOil(_currentOn.Down);
        SetTargetToOil(_currentOn.Left);
        SetTargetToOil(_currentOn.Right);
    }

    private void SetTargetToOil(BaseUnit targetUnit)
    {
        if (targetUnit != null &&
            (targetUnit.myState.stateType == Enum.ENUM_State.Ground || targetUnit.myState.stateType == Enum.ENUM_State.Water))
        {
            targetUnit.myState.OnStateEnd();
            targetUnit.SetState(new Oil(targetUnit));
        }
    }
}
