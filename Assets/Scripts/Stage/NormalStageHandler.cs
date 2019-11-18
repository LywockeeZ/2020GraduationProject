using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 具体的关卡类，含有对关卡内容和关卡分数（过关条件）的引用
/// </summary>
public class NormalStageHandler : IStageHandler
{

    public int RoundActionPts = 3;

    public NormalStageData m_StatgeData = null;       //关卡的内容，负责更新
    public NormalStageScore m_StageScore = null;      //关卡的条件，负责判断
    public IStageHandler m_NextHandler = null;   //下一个关卡
    public int Rounds = 1;

    //设置下一个关卡
    public override IStageHandler SetNextHandler(IStageHandler NextHandler)
    {
        m_NextHandler = NextHandler;
        return m_NextHandler;
    }


    public NormalStageHandler(NormalStageScore StageScore, NormalStageData StageData)
    {
        m_StageScore = StageScore;
        m_StatgeData = StageData;
        m_StageScore.SetStageHandler(this);     //设置其对当前关卡的引用
        m_StatgeData.SetStageHandler(this);
    }

    /// <summary>
    /// 判断关卡是否结束,并返回下一关
    /// </summary>
    /// <returns></returns>
    public override IStageHandler CheckStage()
    {
        if (m_StageScore.CheckStage())
            return m_NextHandler;
        else
            return this;
    }

    public override void Update()
    {
        Debug.Log("AP:"+Game.Instance.GetCurrentAP());
        Debug.Log("IsCanInput:"+Game.Instance.GetCanInput());

        m_StageScore.CheckRound();
    }

    public override void Reset()
    {

    }

    public override void Start()
    {
        m_StatgeData.BuildStage();
        Game.Instance.SetMaxAP(RoundActionPts);
    }


    public override BaseUnit GetBaseUnit(int x, int y)
    {
        return m_StatgeData.GetBaseUnit(x, y);
    }

}
