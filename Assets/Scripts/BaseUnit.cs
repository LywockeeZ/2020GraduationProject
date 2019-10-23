using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit 
{
    //对周围单元的引用
    public BaseUnit Up { get { return _up; } }
    public BaseUnit Down { get { return _down; } }
    public BaseUnit Left { get { return _left; } }
    public BaseUnit Right { get { return _right; } }                                                                                                
    //该单元是否可通行
    public bool CanWalk { get { return _canWalk; } }
    //是否可被点燃
    public bool CanBeFire { get { return _canBeFire; } }
    //自身状态
    public State myState { get { return _myState; } }
    private State _myState;
    public GameObject Model = null;
    //放置单元的类型
    public Enum.ENUM_UpperUnitType UpperType = Enum.ENUM_UpperUnitType.NULL;
    public UpperUnit upperUnit = null;

    //关卡信息的引用，用来获取周围信息
    private Stage currentStage;
    //此单元的位置信息
    private int x = 0;
    private int y = 0;

    private BaseUnit _up;
    private BaseUnit _down;
    private BaseUnit _left;
    private BaseUnit _right;
    private bool _canWalk;
    private bool _canBeFire;

    public BaseUnit(GameObject _model, Stage _stage)
    {
        currentStage = _stage;
        Model = _model;
        Init();
    }


    //单元初始化
    public virtual void Init()
    {
        SetState(new Ground(this));
    }

    //获取周围信息,通过在List中的位置获取
    public virtual void GetAround()
    {
        if (y + 2 > currentStage.Row) _up = null; 
            else _up = currentStage.baseUnits[currentStage.Column * (y + 1) + x];
        if (y - 1 < 0) _down = null;
            else _down = currentStage.baseUnits[currentStage.Column * (y - 1) + x];
        if (x - 1 < 0) _left = null;
            else _left = currentStage.baseUnits[currentStage.Column * y + x - 1];
        if (x + 2 > currentStage.Column) _right = null;
            else _right = currentStage.baseUnits[currentStage.Column * y + x + 1];
    }

    //外界用此来执行状态
    public virtual void StateRequest()
    {
        myState.OnStateHandle();
    }

    public virtual void SetState(State newState)
    {
        if (_myState != null)
        {
            _myState.OnStateEnd();
        }
        _myState = newState;
    }

    public void SetCanWalk(bool value)
    {
        _canWalk = value;
    }

    public void SetCanBeFire(bool value)
    {
        _canBeFire = value;
    }


    public void SetPosition(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public Stage GetStage()
    {
        return currentStage;
    }

    public void SetUpperType(Enum.ENUM_UpperUnitType type)
    {
        UpperType = type;
    }

    public void SetUpperUnit(UpperUnit _upperUnit)
    {
        upperUnit = _upperUnit;
    }

}
