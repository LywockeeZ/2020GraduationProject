using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定义可以处理“过关判断”和“关卡开启”的接口,
/// 将判断条件与关卡数据类化使类能够自由组合
/// </summary>
public abstract class IStageHandler 
{

    public abstract IStageHandler SetNextHandler(IStageHandler NextHandler);
    public abstract IStageHandler CheckStage();
    public abstract void Update();
    public abstract void Reset();
    public abstract void BuildStage();
    public abstract BaseUnit GetBaseUnit(int x, int y);
}
