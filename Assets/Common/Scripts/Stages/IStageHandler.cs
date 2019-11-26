using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡具体数据与通关条件的所有者，是真正的关卡
/// 将判断条件与关卡数据类化使类能够自由组合
/// </summary>
public abstract class IStageHandler 
{

    public abstract IStageHandler SetNextHandler(IStageHandler NextHandler);
    public abstract IStageHandler CheckStage();

    public abstract void Update();
    public abstract void Reset();
    public abstract void Start();

    public abstract BaseUnit GetBaseUnit(int x, int y);
}
