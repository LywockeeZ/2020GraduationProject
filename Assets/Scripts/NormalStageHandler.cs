using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 具体的关卡类，含有对关卡内容和关卡分数（过关条件）的引用
/// </summary>
public class NormalStageHandler : IStageHandler
{
    public NormalStageHandler(IStageScore StageScore, IStageData StageData)
    {
        m_StageScore = StageScore;
        m_StatgeData = StageData;
        m_StageScore.SetStageHandler(this);     //设置其对当前关卡的引用
        m_StatgeData.SetStageHandler(this);
    }

    public override IStageHandler CheckStage()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {

    }

    public override void Reset()
    {

    }

    public override bool IsFinished()
    {
        throw new System.NotImplementedException();
    }

    public override BaseUnit GetBaseUnit(int x, int y)
    {
        return m_StatgeData.GetBaseUnit(x, y);
    }

}
