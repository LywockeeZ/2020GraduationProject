using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class APSystem : IGameSystem
{
    public int ActionPts = 0;       //当前行动点数
    public int Round_MAX_ActionPts = 0;      //每回合最大行动点数
    public int LastRoundRemainPts = 0;      //上回合未扣除的点数
    public int CostTotalPts = 0;        //花费的总点数
    public int Rounds = 1;       //当前回合数,由外部具体关卡设置


    public APSystem()
    {
        Initialize();
    }



    public override void Initialize()
    {
        RegisterEvent();
    }


    private EventListenerDelegate OnStageBegain;
    private EventListenerDelegate OnStageRestart;
    private EventListenerDelegate RoundBegain;

    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageBegain,
        OnStageBegain = (Message evt) =>
        {
            ResetActionPoints();
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.StageRestart,
        OnStageRestart = (Message evt) =>
        {
            ResetAPSystem();
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundBegain,
        RoundBegain =  (Message evt) =>
        {
            ResetActionPoints();
            //等界面加载之后再设置ActionBar
            Action call = () => {
                Game.Instance.SetActionBar(ActionPts);
            };
            CoroutineManager.StartCoroutineTask(call, 0.01f);
            //GUIManager.Instance.SetAPBarImage(GetCurrentAP());
            //GUIManager.Instance.SetAPText(GetCurrentAP());
            //GUIManager.Instance.SetCostAPText(CostTotalPts);
        });

    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.StageBegain, OnStageBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageRestart, OnStageRestart);
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundBegain, RoundBegain);
    }

    public override void Release()
    {
        DetachEvent();
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

        Game.Instance.SetActionBar(ActionPts);
        //GUIManager.Instance.SetAPText(GetCurrentAP());
        //GUIManager.Instance.SetCostAPText(CostTotalPts);
        //GUIManager.Instance.SetAPBarImage(GetCurrentAP());

    }




    public bool CostAP(int value, int additionValue)
    {
        if (value > ActionPts)
            return false;
        else
        {
            ReducePoints(value, additionValue);
            return true;
        }
    }



    
    public void SetRoundActionPts(int value)
    {
        ActionPts = value;
        Round_MAX_ActionPts = value;
    }




    public void ResetActionPoints()
    {
        ActionPts = Round_MAX_ActionPts;
    }




    public void ResetAPSystem()
    {
        ActionPts = 0;       
        Round_MAX_ActionPts = 0;     
        LastRoundRemainPts = 0;      
        CostTotalPts = 0;        
    }




    public int GetCurrentAP()
    {
        return ActionPts;
    }

    
    public int GetTotalCostPts()
    {
        return CostTotalPts;
    }

}
