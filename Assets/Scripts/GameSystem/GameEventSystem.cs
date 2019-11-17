using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : IGameSystem
{
    private Dictionary<Enum.ENUM_GameEvent, IGameEventSubject> m_GameEvents
            = new Dictionary<Enum.ENUM_GameEvent, IGameEventSubject>();

    public GameEventSystem()
    {
        Initialize();
    }

    public override void Release()
    {
        m_GameEvents.Clear();
    }

    public void RegisterObserver(Enum.ENUM_GameEvent emGameEvent,
                                 IGameEventObserver Observer)
    {
        //获取事件
        IGameEventSubject Subject = GetGameEventSubject(emGameEvent);
        if (Subject != null)
        {
            Subject.Attach(Observer);
            Observer.SetSubject(Subject);
        }
    }

    // 注册一个事件
    private IGameEventSubject GetGameEventSubject(Enum.ENUM_GameEvent emGameEvent)
    {
        if (m_GameEvents.ContainsKey(emGameEvent))
            return m_GameEvents[emGameEvent];

        //产生对应的GameEvent
        IGameEventSubject pSujbect = null;
        switch (emGameEvent)
        {
            case Enum.ENUM_GameEvent.NewStage:
                pSujbect = new NewStageSubject();
                break;
            case Enum.ENUM_GameEvent.NewRound:
                pSujbect = new NewRoundSubject();
                break;
            case Enum.ENUM_GameEvent.RoundUpdateBegain:
                pSujbect = new RoundUpdateBegainSubject();
                break;
            case Enum.ENUM_GameEvent.RoundUpdateEnd:
                pSujbect = new RoundUpdateEndSubject();
                break;
            case Enum.ENUM_GameEvent.StageEnd:
                break;
            default:
                break;
        }

        //加入并返回
        m_GameEvents.Add(emGameEvent, pSujbect);
        return pSujbect;
    }

    public void NotifySubject(Enum.ENUM_GameEvent emGameEvent, System.Object Param)
    {
        if (m_GameEvents.ContainsKey(emGameEvent) == false)
            return;
        m_GameEvents[emGameEvent].SetParam(Param);
    }
}
