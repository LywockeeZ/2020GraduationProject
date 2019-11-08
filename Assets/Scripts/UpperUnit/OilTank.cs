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
        //_currentOn.myState.OnStateEnd();
        //_currentOn.SetState(new Ground(_currentOn));
        _currentOn.SetUpperType(Enum.ENUM_UpperUnitType.Fixed);
        _currentOn.SetUpperGameObject(gameObject);

        transform.position = SetTargetPos(transform.position);

    }


    public void Handle()
    {
        GameManager.Instance.ReducePoints(1, 0);
        _currentOn.myState.OnStateEnd();
        _currentOn.SetState(new Oil(_currentOn));
        SetAroundToOil();
        End();
    }

    public void HandleByFire()
    {
        _currentOn.SetState(new Fire(_currentOn));
        GameManager.Instance.StartCoroutine(BoomAccess());
        End();
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

    private void Boom()
    {
        List<BaseUnit> units = GetBoomRangeAround();
        foreach (var unit in units)
        {
            if (unit != null )
            {
                if (unit.UpperType == Enum.ENUM_UpperUnitType.NULL)
                {
                    if (unit.myState.stateType != Enum.ENUM_State.Block)
                    {
                        unit.myState.OnStateEnd();
                        unit.SetState(new Fire(unit));
                    }
                }
                else
                if (unit.UpperType == Enum.ENUM_UpperUnitType.Fixed)
                {
                    unit.UpperGameObject.GetComponent<IFixedUnit>().HandleByFire();
                }
            }
        }
    }

    private void FireSpread()
    {
        List<BaseUnit> units = GetBoomRangeAdditionAround();
        List<BaseUnit> FireNeighbor = new List<BaseUnit>();
        foreach (var unit in units)
        {
            if (unit != null)
            {
                if (unit.UpperType == Enum.ENUM_UpperUnitType.NULL)
                {
                    if (unit.myState.stateType == Enum.ENUM_State.Fire)
                    {
                        FireNeighbor.Add(unit);
                    }
                }
            }
        }
        foreach (var unit in FireNeighbor)
        {
            unit.myState.OnStateHandle();
        }

    }

    //获取爆炸范围的信息
    private List<BaseUnit> GetBoomRangeAround()
    {
        Stage _currentStage = CurrentOn.GetStage();
        List<BaseUnit> units = new List<BaseUnit>();
        if (CurrentOn.y + 1 <= _currentStage.Column && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x - 1]);
        if (CurrentOn.y + 1 <= _currentStage.Column && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x    ]);
        if (CurrentOn.y + 1 <= _currentStage.Column && CurrentOn.x + 1 <= _currentStage.Row)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x + 1]);
        if (CurrentOn.y     >= 0 && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x - 1]);
        if (CurrentOn.y     >= 0 && CurrentOn.x + 1 <= _currentStage.Row)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x + 1]);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x - 1]);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x    ]);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x + 1 <= _currentStage.Row)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x + 1]);
        return units;
    }

    //爆炸范围外围信息
    private List<BaseUnit> GetBoomRangeAdditionAround()
    {
        Stage _currentStage = CurrentOn.GetStage();
        List<BaseUnit> units = new List<BaseUnit>();
        if (CurrentOn.y + 2 <= _currentStage.Column && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x - 2]);
        if (CurrentOn.y + 2 <= _currentStage.Column && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x - 1]);
        if (CurrentOn.y + 2 <= _currentStage.Column && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x    ]);
        if (CurrentOn.y + 2 <= _currentStage.Column && CurrentOn.x + 1 <= _currentStage.Row)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x + 1]);
        if (CurrentOn.y + 2 <= _currentStage.Column && CurrentOn.x + 2 <= _currentStage.Row)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x + 2]);

        if (CurrentOn.y + 1 <= _currentStage.Column && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x - 2]);
        if (CurrentOn.y + 1 <= _currentStage.Column && CurrentOn.x + 2 <= _currentStage.Row)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x + 2]);

        if (CurrentOn.y     <= _currentStage.Column && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x - 2]);
        if (CurrentOn.y     <= _currentStage.Column && CurrentOn.x + 2 <= _currentStage.Row)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x + 2]);

        if (CurrentOn.y - 1 >= 0 && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x - 2]);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x + 2 <= _currentStage.Row)

            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x + 2]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x - 2]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x - 1]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x    ]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x + 1 <= _currentStage.Row)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x + 1]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x + 2 <= _currentStage.Row)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x + 2]);

        return units;
    }

    IEnumerator BoomAccess()
    {
        yield return new WaitForSeconds(0.1f);
        Boom();
        yield return new WaitForSeconds(1f);
        FireSpread();
        GameManager.Instance.StopCoroutine(BoomAccess());
    }
}
