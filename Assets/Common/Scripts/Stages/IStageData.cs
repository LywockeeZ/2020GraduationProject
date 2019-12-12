using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责具体的关卡内容
/// </summary>
public abstract class IStageData
{

    public abstract void Update();      //每个关卡内容上的更新
    public abstract void BuildStage(int startPosX, int startPosY);
    public abstract void Reset();       //重置关卡
    public abstract BaseUnit GetBaseUnit(int x, int y);     //获得坐标对应的单元

}
