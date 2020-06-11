using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HighlightingSystem;

/// <summary>
/// 存放上层单元信息的结构体，包括单元类型，玩家操控类型，火焰触发类型
/// </summary>
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



public class BaseUnit : MonoBehaviour
{
    //对周围单元的引用
    public BaseUnit Up    { get { return _up   ; } }
    public BaseUnit Down  { get { return _down ; } }
    public BaseUnit Left  { get { return _left ; } }
    public BaseUnit Right { get { return _right; } }      
    

    //单元属性
    public bool CanWalk   { get { return _canWalk  ; } }            //该单元是否可通行
    public bool CanBeFire { get { return _canBeFire; } }            //是否可被点燃
    public IState State   { get { return _myState  ; } }            //自身状态
                                                                    
    //此单元的位置信息
    public int x { get { return _x; } }                             
    public int y { get { return _y; } }

    public GameObject Model           = null;
    public GameObject UpperGameObject = null;
    /// <summary>
    /// 描述此单元上层放置单元的属性，是一个结构体
    /// 使外界方便获取上层单元信息
    /// 由上层单元初始化时赋值
    /// </summary>
    public UpperUnit UpperUnit;


    #region 私有属性
    private BaseUnit _up;
    private BaseUnit _down;
    private BaseUnit _left;
    private BaseUnit _right;
    private IState _myState;
    private bool _canWalk;
    private bool _canBeFire;
    private int _x = 0;
    private int _y = 0;
    /// <summary>
    /// 关卡信息的引用，用来获取周围信息
    /// </summary>
    private NormalStageData currentStage;

    //析构函数
    //~BaseUnit()
    //{
    //    Debug.Log("已销毁");
    //}

    #endregion

    /// <summary>
    /// 外部调用用来初始化单元
    /// </summary>
    public void BaseUnitInit(GameObject _model, NormalStageData _stage)
    {
        currentStage = _stage;
        Model = _model;
        UpperUnit.InitOrReset();
        Init();
    }



    /// <summary>
    /// 单元的初始化
    /// </summary>
    public virtual void Init()
    {
        //需要改动
        SetState(new Ground(this)); 
    }


    /// <summary>
    /// 结束一个单元：结束状态，结束上层单元，回收模型
    /// </summary>
    public virtual void End()
    {
        State.OnStateEnd();
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(Model);

        if (UpperGameObject != null)
            GameFactory.GetAssetFactory().DestroyGameObject<IUpperUnit>(UpperGameObject);

        UpperGameObject = null;
        ClearAround();
        transform.GetChild(0).GetComponent<Highlighter>().ConstantOffImmediate();
        Destroy(this);
    }


    /// <summary>
    /// 外界由此来执行状态的行为
    /// </summary>
    public virtual void StateRequest()
    {
        State.OnStateHandle();
    }


    /// <summary>
    /// 外界由此控制状态结束
    /// </summary>
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

        //Debug.Log("UP:" + _up + "Down:" + _down + "Left:" + _left + "Right:" + _right);
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



    #region 对外的接口
    /// <summary>
    /// 设置单元状态
    /// </summary>
    /// <param name="newState"></param>
    public virtual void SetState(IState newState)
    {
        if (_myState != null)
        {
            _myState.OnStateEnd();
        }
        _myState = newState;
    }


    /// <summary>
    /// 设置单元状态,结束后调用回调
    /// </summary>
    /// <param name="newState"></param>

    public virtual void SetState(IState newState, Action callBack)
    {
        if (_myState != null)
        {
            _myState.OnStateEnd(callBack);
        }
        _myState = newState;
    }


    /// <summary>
    /// 设置表面是否可以通行
    /// </summary>
    /// <param name="value"></param>
    public void SetCanWalk(bool value)
    {
        _canWalk = value;
    }


    /// <summary>
    /// 设置单元是否可被点燃
    /// </summary>
    /// <param name="value"></param>
    public void SetCanBeFire(bool value)
    {
        _canBeFire = value;
    }


    /// <summary>
    /// 设置上层单元位置信息
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    public void SetPosition(int Posx, int Posy)
    {
        _x = Posx;
        _y = Posy;
    }


    /// <summary>
    /// 获取单元所在关卡信息
    /// </summary>
    /// <returns></returns>
    public NormalStageData GetStage()
    {
        return currentStage;
    }


    /// <summary>
    /// 设置上层游戏对象
    /// </summary>
    /// <param name="_upperGameObject"></param>
    public void SetUpperGameObject(GameObject _upperGameObject)
    {
        UpperGameObject = _upperGameObject;
    }
    #endregion
}
