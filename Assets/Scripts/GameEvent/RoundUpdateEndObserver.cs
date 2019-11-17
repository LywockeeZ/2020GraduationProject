using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundUpdateEndObserver : IGameEventObserver
{
    IGameEventSubject m_Subject = null;
    Action callBack = null;

    public override void SetSubject(IGameEventSubject Subject)
    {
        m_Subject = Subject as RoundUpdateBegainSubject;
    }

    public RoundUpdateEndObserver(Action _callBack)
    {
        callBack = _callBack;
    }



    public override void Update()
    {
        callBack();
    }

}
