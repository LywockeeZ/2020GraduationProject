using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundUpdateBegainStageDataObserver : IGameEventObserver
{
    IGameEventSubject m_Subject = null;
    Func<bool> callBack = null;

    public override void SetSubject(IGameEventSubject Subject)
    {
        m_Subject = Subject as RoundUpdateBegainSubject;
    }

    public RoundUpdateBegainStageDataObserver(Func<bool> _callBack)
    {
        callBack = _callBack;
    }



    public override void Update()
    {
        CoroutineManager.StartCoroutineTask(callBack, Enum.ENUM_GameEvent.RoundUpdateEnd, 1f);
    }
}
