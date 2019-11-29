using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilTank : MonoBehaviour, IUpperUnit, IFixedUnit, ICanBeFiredUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float    Height    { get { return _heigth   ; } set { _heigth    = value; } }
    public bool     CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }

    private ENUM_UpperUnit Type = ENUM_UpperUnit.OilTank;                             //放置单元的类型
    private ENUM_UpperUnitControlType ControlType = ENUM_UpperUnitControlType.Fixed;  //放置单元的操控类型
    private ENUM_UpperUnitBeFiredType BeFiredType = ENUM_UpperUnitBeFiredType.BeFire; 

    public Animator animator;

    private BaseUnit _currentOn;
    private float    _heigth = 0f;
    private bool     _canBeFire = true;



    private void Start()
    {
        Init();
    }



    public void Init()
    {
        //初始化状态
        _currentOn.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
        _currentOn.SetUpperGameObject(gameObject);
        _currentOn.SetCanBeFire(_canBeFire);

        transform.position = SetTargetPos(transform.position);
        animator = transform.GetChild(0).GetComponent<Animator>();

    }



    public void Handle()
    {
        Game.Instance.CostAP(1, 0);

        _currentOn.StateEnd();
        _currentOn.SetState(new Oil(_currentOn));

        SetAroundToOil();
        if (animator != null) animator.SetTrigger("Break");

        End();
    }



    public void HandleByFire()
    {
        _currentOn.SetState(new Fire(_currentOn));
        CoroutineManager.StartCoroutine(BoomAccess());

        if (animator != null) animator.SetTrigger("Break");

        End();
    }



    public void End()
    {
        _currentOn.UpperUnit.InitOrReset();
    }



    //将单元的高度转换为模型高度
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _heigth, _targetPos.z);
    }



    private void SetAroundToOil()
    {
        List<BaseUnit> units = GetBoomRangeAround();
        if (CurrentOn.Up != null )
        {
            SetTargetToOil(units[0]);
            SetTargetToOil(units[1]);
            SetTargetToOil(units[2]);
        }
        if (CurrentOn.Down != null )
        {
            SetTargetToOil(units[5]);
            SetTargetToOil(units[6]);
            SetTargetToOil(units[7]);
        }
        if (CurrentOn.Left != null )
        {
            SetTargetToOil(units[0]);
            SetTargetToOil(units[3]);
            SetTargetToOil(units[5]);
        }
        if (CurrentOn.Right != null )
        {
            SetTargetToOil(units[2]);
            SetTargetToOil(units[4]);
            SetTargetToOil(units[7]);
        }


    }



    private void SetTargetToOil(BaseUnit targetUnit)
    {
        if (targetUnit != null &&
            (targetUnit.State.stateType == ENUM_State.Ground || targetUnit.State.stateType == ENUM_State.Water || targetUnit.State.stateType == ENUM_State.Block))
        {
            targetUnit.StateEnd();
            targetUnit.SetState(new Oil(targetUnit));
        }
    }




    private void SetTargetToFire(BaseUnit targetUnit)
    {
        if (targetUnit.UpperGameObject != null)
        {
            if (targetUnit.UpperUnit.BeFiredType != ENUM_UpperUnitBeFiredType.BeFire)
            {
                return;
            }
            else
            if (targetUnit.UpperUnit.BeFiredType == ENUM_UpperUnitBeFiredType.BeFire)
            {
                targetUnit.UpperGameObject.GetComponent<ICanBeFiredUnit>().HandleByFire();
            }
            else
            if (targetUnit.UpperUnit.Type == ENUM_UpperUnit.Player)
            {
                targetUnit.StateEnd();
                targetUnit.SetState(new Block(targetUnit));
            }
        }
        else
        {
            if (targetUnit.State.beFiredType == ENUM_StateBeFiredType.BeFire)
            {
                targetUnit.StateEnd();
                targetUnit.SetState(new Fire(targetUnit));
            }
            else
            if (targetUnit.State.beFiredType == ENUM_StateBeFiredType.BeHandle)
            {
                targetUnit.StateRequest();
            }
        }
    }




    private void Boom()
    {
        List<BaseUnit> units = GetBoomRangeAround();
        foreach (var unit in units)
        {
            SetTargetToFire(unit);
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
                if (unit.UpperUnit.Type == ENUM_UpperUnit.NULL)
                {
                    if (unit.State.stateType == ENUM_State.Fire)
                    {
                        FireNeighbor.Add(unit);
                    }
                }
            }
        }
        foreach (var unit in FireNeighbor)
        {
            unit.StateRequest();
        }
        _currentOn.GetStage().isOilUpdateEnd = true;
    }




    //获取爆炸范围的信息
    private List<BaseUnit> GetBoomRangeAround()
    {
        NormalStageData _currentStage = CurrentOn.GetStage();
        List<BaseUnit> units = new List<BaseUnit>();
        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x - 1]);
        else units.Add(null);
        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x    ]);
        else units.Add(null);
        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x + 1]);
        else units.Add(null);
        if (CurrentOn.y     >= 0 && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x - 1]);
        else units.Add(null);
        if (CurrentOn.y     >= 0 && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x + 1]);
        else units.Add(null);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x - 1]);
        else units.Add(null);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x    ]);
        else units.Add(null);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x + 1]);
        else units.Add(null);
        return units;
    }



    //爆炸范围外围信息
    private List<BaseUnit> GetBoomRangeAdditionAround()
    {
        NormalStageData _currentStage = CurrentOn.GetStage();
        List<BaseUnit> units = new List<BaseUnit>();
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x - 2]);
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x - 1]);
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x    ]);
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x + 1]);
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x + 2]);

        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x - 2]);
        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x + 2]);

        if (CurrentOn.y     < _currentStage.Row && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x - 2]);
        if (CurrentOn.y     < _currentStage.Row && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x + 2]);

        if (CurrentOn.y - 1 >= 0 && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x - 2]);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x + 2]);

        if (CurrentOn.y - 2 >= 0 && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x - 2]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x - 1]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x    ]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x + 1]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x + 2]);

        return units;
    }



    IEnumerator BoomAccess()
    {
        _currentOn.GetStage().isOilUpdateEnd = false;
        yield return new WaitForSeconds(0.1f);
        Boom();
        yield return new WaitForSeconds(1f);
        FireSpread();
        CoroutineManager.StopCoroutine(BoomAccess());
    }
}
