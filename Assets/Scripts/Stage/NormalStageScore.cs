using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStageScore : IStageScore
{
    public int ActionPts = 0;       //当前行动点数
    public int RoundActionPts = 0;      //每回合最大行动点数
    public int LastRoundRemainPts = 0;      //上回合未扣除的点数
    public int CostTotalPts = 0;        //花费的总点数
    public int Rounds = 0;



    private bool isRoundShouldUpdate = false;


    public NormalStageHandler m_StageHandler = null;

    public void SetStageHandler(NormalStageHandler stageHandler)
    {
        m_StageHandler = stageHandler;
    }

    /// <summary>
    /// 判断关卡结束
    /// </summary>
    /// <returns></returns>
    public override bool CheckScore()
    {
        NormalStageData normalStageData = m_StageHandler.m_StatgeData;
        bool isOver = false;
        if (normalStageData.GetFireCounts() == 0)
        {
            isOver = true;
        }

        return isOver;
    }


    public void SetRoundActionPts(int value)
    {
        ActionPts = value;
        RoundActionPts = value;
    }


    public void ResetActionPoints()
    {
        ActionPts = RoundActionPts;
    }


    /// <summary>
    /// 扣除行动点数,当扣除之后行动点数为0时,额外动点数将留到下回合
    /// </summary>
    /// <param name="value"></param>
    /// <param name="additionValue"></param>
    public void ReducePoints(int value, int additionValue)
    {
        ActionPts -= value;
        CostTotalPts += value;
        if (ActionPts == 0 && additionValue != 0)
        {
            LastRoundRemainPts = additionValue;
        }
        else
        {
            ActionPts -= additionValue;
            CostTotalPts += additionValue;
        }
        Debug.Log("ActionPoints:" + ActionPts);
    }



    public bool IsRoundShouldUpdata()
    {
        if (ActionPts == 0)
        {
            isRoundShouldUpdate = true;

        }
        else isRoundShouldUpdate = false;

        return isRoundShouldUpdate;
    }

}
