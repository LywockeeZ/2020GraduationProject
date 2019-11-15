using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责具体的关卡内容
/// </summary>
public abstract class IStageData
{
    public IStageHandler m_StageHandler = null;

    public abstract void Update();      //每个关卡内容上的更新
    public abstract void BuildStage();
    public abstract bool IsFinished();  //判定关卡内容是否布置完毕
    public abstract void Reset();       //重置关卡
    public abstract BaseUnit GetBaseUnit(int x, int y);     //获得坐标对应的单元

    public void SetStageHandler(IStageHandler stageHandler)
    {
        m_StageHandler = stageHandler;
    }
}
