using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制火势的关卡
/// </summary>
public class FireControlStageHandler : IStageHandler
{
    //设置关卡分数及关卡数据
    public FireControlStageHandler(IStageScore StageScore, IStageData StageDate)
    {
        m_StageScore = StageScore;
        m_StatgeData = StageDate;
    }


    public override IStageHandler CheckStage()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
    }


    public override bool IsFinished()
    {
        throw new System.NotImplementedException();
    }

    public override void Reset()
    {
    }

    public override void BuildStage()
    {
        throw new System.NotImplementedException();
    }

    public override BaseUnit GetBaseUnit(int x, int y)
    {
        throw new System.NotImplementedException();
    }
}
