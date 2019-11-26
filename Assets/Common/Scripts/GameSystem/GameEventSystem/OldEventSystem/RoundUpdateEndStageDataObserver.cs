using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundUpdateEndObserverStageData : IGameEventObserver
{
    RoundUpdateEndSubject m_Subject = null;
    Action callBack = null;

    public override void SetSubject(IGameEventSubject Subject)
    {
        m_Subject = Subject as RoundUpdateEndSubject;
    }

    public RoundUpdateEndObserverStageData(Action _callBack)
    {
        callBack = _callBack;
    }



    public override void Update()
    {
        callBack();
    }

}
