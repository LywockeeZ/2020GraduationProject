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


    public APSystem()
    {
        Initialize();
    }



    public override void Initialize()
    {
        RegisterEvent();
    }



    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageBegain,
        delegate (Message evt)
        {
            ResetActionPoints();
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundBegain,
        delegate (Message evt)
        {
            ResetActionPoints();
        });

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


}
