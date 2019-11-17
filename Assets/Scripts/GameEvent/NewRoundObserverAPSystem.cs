using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NewRoundObserverAPSystem : IGameEventObserver
{
    NewRoundSubject m_Subject = null;
    Action callBack = null;

    public override void SetSubject(IGameEventSubject Subject)
    {
        m_Subject = Subject as NewRoundSubject;
    }

    public NewRoundObserverAPSystem(Action _callBack)
    {
        callBack = _callBack;
    }



    public override void Update()
    {
        callBack();
    }
}
