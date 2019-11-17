using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStageScore : IStageScore
{
    private bool isRoundShouldUpdate = false;


    public NormalStageHandler m_StageHandler = null;
    public void SetStageHandler(NormalStageHandler stageHandler)
    {
        m_StageHandler = stageHandler;
    }

    public NormalStageScore()
    {
    }

    /// <summary>
    /// 判断关卡结束
    /// </summary>
    /// <returns></returns>
    public override bool CheckScore()
    {
        bool isOver = false;
        if (m_StageHandler.m_StatgeData.GetFireCounts() == 0)
        {
            isOver = true;
        }

        return isOver;
    }




    public bool IsRoundShouldUpdate()
    {
        if (Game.Instance.GetCurrentAP() == 0)
        {
            isRoundShouldUpdate = true;
        }
        else isRoundShouldUpdate = false;

        return isRoundShouldUpdate;
    }

    public void ResetIsShouldUpdate()
    {
        isRoundShouldUpdate = false;
    }

}
