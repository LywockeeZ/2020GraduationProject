using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundUpdateEndObserverUI : IGameEventObserver
{
    RoundUpdateEndSubject m_Subject = null;
    Action callBack = null;

    public override void SetSubject(IGameEventSubject Subject)
    {
        m_Subject = Subject as RoundUpdateEndSubject;
    }

    public RoundUpdateEndObserverUI(Action _callBack)
    {
        callBack = _callBack;
    }

    public override void Update()
    {
        //如果关卡结束，则调用相应的UI界面
        if (m_Subject.isStageEnd)
        {
            callBack();
        }
    }
}
