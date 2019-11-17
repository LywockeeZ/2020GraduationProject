using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundUpdateBegainObserverStageData : IGameEventObserver
{
    IGameEventSubject m_Subject = null;
    Func<bool> callBack = null;

    public override void SetSubject(IGameEventSubject Subject)
    {
        m_Subject = Subject as RoundUpdateBegainSubject;
    }

    public RoundUpdateBegainObserverStageData(Func<bool> _callBack)
    {
        callBack = _callBack;
    }



    public override void Update()
    {
        //回合更新结束自动触发事件
        CoroutineManager.StartCoroutineTask(callBack, Enum.ENUM_GameEvent.RoundUpdateEnd, 1f);
    }
}
