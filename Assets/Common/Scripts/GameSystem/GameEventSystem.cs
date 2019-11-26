using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : IGameSystem
{
    //private Dictionary<ENUM_GameEvent, IGameEventSubject> m_GameEvents
    //        = new Dictionary<ENUM_GameEvent, IGameEventSubject>();

    public GameEventSystem()
    {
        Initialize();
    }

    public override void Release()
    {
        //m_GameEvents.Clear();
    }

    //public void RegisterObserver(ENUM_GameEvent emGameEvent,
    //                             IGameEventObserver Observer)
    //{
    //    //获取事件
    //    IGameEventSubject Subject = GetGameEventSubject(emGameEvent);
    //    if (Subject != null)
    //    {
    //        Subject.Attach(Observer);
    //        Observer.SetSubject(Subject);
    //    }
    //}

    //// 注册一个事件
    //private IGameEventSubject GetGameEventSubject(ENUM_GameEvent emGameEvent)
    //{
    //    if (m_GameEvents.ContainsKey(emGameEvent))
    //        return m_GameEvents[emGameEvent];

    //    //产生对应的GameEvent
    //    IGameEventSubject pSujbect = null;
    //    switch (emGameEvent)
    //    {
    //        case ENUM_GameEvent.NewStage:
    //            pSujbect = new NewStageSubject();
    //            break;
    //        case ENUM_GameEvent.NewRound:
    //            pSujbect = new NewRoundSubject();
    //            break;
    //        case ENUM_GameEvent.RoundUpdateBegain:
    //            pSujbect = new RoundUpdateBegainSubject();
    //            break;
    //        case ENUM_GameEvent.RoundUpdateEnd:
    //            pSujbect = new RoundUpdateEndSubject();
    //            break;
    //        case ENUM_GameEvent.StageEnd:
    //            break;
    //        default:
    //            break;
    //    }

    //    //加入并返回
    //    m_GameEvents.Add(emGameEvent, pSujbect);
    //    return pSujbect;
    //}

    //public void NotifySubject(ENUM_GameEvent emGameEvent, System.Object Param)
    //{
    //    if (m_GameEvents.ContainsKey(emGameEvent) == false)
    //        return;
    //    m_GameEvents[emGameEvent].SetParam(Param);
    //}

    public void RegisterEvent(ENUM_GameEvent type, EventListenerDelegate listener )
    {
        Dispatcher.Instance.AddListener(type, listener);
    }

    public void DetachEvent(ENUM_GameEvent type, EventListenerDelegate listener)
    {
        Dispatcher.Instance.RemoveListener(type, listener);
    }

    public void NotifyEvent(ENUM_GameEvent type, params System.Object[] param)
    {
        Dispatcher.Instance.SendMessage(type, param);
    }

    public void NotifyEvent(Message evt)
    {
        Dispatcher.Instance.SendMessage(evt);
    }
}
