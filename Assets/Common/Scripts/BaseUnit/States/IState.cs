using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 在构造函数中初始化自身属性，在OnStateBegain中初始化状态拥有者属性
 */

public abstract class IState
{
    /// <summary>
    /// 表示该状态的枚举
    /// </summary>
    public ENUM_State StateType;
    /// <summary>
    /// 该状态被火焰触发的操作，点燃/触发操作
    /// </summary>
    public ENUM_StateBeFiredType BeFiredType;
    /// <summary>
    /// 状态所用的模型
    /// </summary>
    public GameObject Model;
    /// <summary>
    /// 状态持有者
    /// </summary>
    protected BaseUnit Owner = null;


    /// <summary>
    /// 调试用
    /// </summary>
    public string StateName { get { return _stateName; } }
    protected string _stateName = null;

    public IState(BaseUnit owner)
    {
        //设置状态拥有者
        Owner = owner;
    }

    /// <summary>
    /// 状态初始化时调用
    /// </summary>
    public abstract void OnStateBegin();
    /// <summary>
    /// 控制状态的行为
    /// </summary>
    public abstract void OnStateHandle();
    /// <summary>
    /// 状态结束时调用
    /// </summary>
    public abstract void OnStateEnd();

    /// <summary>
    /// 返回一个y设置为height的向量
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public Vector3 GetTargetPos(Vector3 pos, float height)
    {
        return new Vector3(pos.x, height, pos.z);
    }

}
