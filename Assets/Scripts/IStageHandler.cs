using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定义可以处理“过关判断”和“关卡开启”的接口
/// </summary>
public abstract class IStageHandler 
{
    protected IStageData m_StatgeData = null;       //关卡的内容
    protected IStageScore m_StageScore = null;      //关卡的分数，通关条件
    protected IStageHandler m_NextHandler = null;   //下一个关卡

    //设置下一个关卡
    public IStageHandler SetNextHandler(IStageHandler NextHandler)
    {
        m_NextHandler = NextHandler;
        return m_NextHandler;
    }

    public abstract IStageHandler CheckStage();
    public abstract void Update();
    public abstract void Reset();
    public abstract bool IsFinished();
    public abstract BaseUnit GetBaseUnit(int x, int y);
}
