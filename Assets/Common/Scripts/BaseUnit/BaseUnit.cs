using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UpperUnit
{
    public ENUM_UpperUnit Type;
    public ENUM_UpperUnitControlType ControlType;
    public ENUM_UpperUnitBeFiredType BeFiredType;

    public UpperUnit(ENUM_UpperUnit _type, 
                     ENUM_UpperUnitControlType _controlType, 
                     ENUM_UpperUnitBeFiredType _beFiredType)
    {
        Type = _type;
        ControlType = _controlType;
        BeFiredType = _beFiredType;
    }

    /// <summary>
    /// 重置或初始化
    /// </summary>
    public void InitOrReset()
    {
        Type = ENUM_UpperUnit.NULL;
        ControlType = ENUM_UpperUnitControlType.NULL;
        BeFiredType = ENUM_UpperUnitBeFiredType.NULL;
    }
}

public class BaseUnit 
{
    //对周围单元的引用
    public BaseUnit Up    { get { return _up   ; } }
    public BaseUnit Down  { get { return _down ; } }
    public BaseUnit Left  { get { return _left ; } }
    public BaseUnit Right { get { return _right; } }      
    

    public bool CanWalk   { get { return _canWalk  ; } }            //该单元是否可通行
    public bool CanBeFire { get { return _canBeFire; } }            //是否可被点燃
    public IState State   { get { return _myState  ; } }            //自身状态


    public GameObject Model           = null;
    public GameObject UpperGameObject = null;
    public UpperUnit UpperUnit;
    //public ENUM_UpperUnit            UpperUnitType    = ENUM_UpperUnit.NULL;             //放置单元的类型
    //public ENUM_UpperUnitControlType UpperControlType = ENUM_UpperUnitControlType.NULL;  //放置单元的操控类型
    //public ENUM_UpperUnitBeFiredType UpperBeFiredType = ENUM_UpperUnitBeFiredType.NULL;  //放置单元的可燃性

    //此单元的位置信息
    public int x = 0;
    public int y = 0;
    private NormalStageData currentStage;       //关卡信息的引用，用来获取周围信息


    private BaseUnit _up;
    private BaseUnit _down;
    private BaseUnit _left;
    private BaseUnit _right;
    private IState _myState;
    private bool _canWalk;
    private bool _canBeFire;

    public BaseUnit(GameObject _model, NormalStageData _stage)
    {
        currentStage = _stage;
        Model = _model;
        UpperUnit.InitOrReset();
        Init();
    }


    //单元初始化
    public virtual void Init()
    {
        //需要改动
        SetState(new Ground(this));
    }


    //外界用此来执行状态
    public virtual void StateRequest()
    {
        State.OnStateHandle();
    }


    public virtual void StateEnd()
    {
        State.OnStateEnd();
    }



    /// <summary>
    /// 获取周围信息,通过在List中的位置获取
    /// </summary>
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


    /// <summary>
    /// 清除对周围信息的引用
    /// </summary>
    public virtual void ClearAround()
    {
        _up = null;
        _down = null;
        _left = null;
        _right = null;
    }

    public virtual void SetState(IState newState)
    {
        if (_myState != null)
        {
            _myState.OnStateEnd();
        }
        _myState = newState;
    }


    #region 对外的接口
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

    public NormalStageData GetStage()
    {
        return currentStage;
    }

    //public void SetUpperControlType(ENUM_UpperUnitControlType type)
    //{
    //    //UpperControlType = type;
    //}

    public void SetUpperGameObject(GameObject _upperGameObject)
    {
        UpperGameObject = _upperGameObject;
    }
    #endregion
}
