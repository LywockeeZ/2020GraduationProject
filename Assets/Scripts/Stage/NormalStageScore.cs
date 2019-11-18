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
        RegisterEvent();
    }



    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundBegain,
        delegate (Message evt)
        {
            Game.Instance.SetCanInput(true);
            ResetIsShouldUpdate();
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundEnd,
        delegate (Message evt)
        {
            //回合结束时判断关卡是否结束
            if (CheckStage())
                Game.Instance.NotifyEvent(ENUM_GameEvent.StageEnd);
            else
                Game.Instance.NotifyEvent(ENUM_GameEvent.RoundBegain);
        });


    }



    /// <summary>
    /// 判断关卡结束
    /// </summary>
    /// <returns></returns>
    public override bool CheckStage()
    {
        bool isOver = false;
        if (m_StageHandler.m_StatgeData.GetFireCounts() == 0)
        {
            isOver = true;
        }

        return isOver;
    }



    /// <summary>
    /// 判断并通知回合的开始更新
    /// 每个关卡第一回合的开始都需要一个按钮来推动
    /// </summary>
    public override void CheckRound()
    {

        if (Game.Instance.GetCurrentAP() == 0 && isRoundShouldUpdate)
        {
            Game.Instance.NotifyEvent(ENUM_GameEvent.RoundUpdateBegain, null);
            isRoundShouldUpdate = false;
        }
    }



    private void ResetIsShouldUpdate()
    {
        isRoundShouldUpdate = true;
    }

}
