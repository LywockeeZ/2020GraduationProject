using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    private static Game _instance;
    public static Game Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Game();
            return _instance;
        }
    }

    private StageSystem m_StageSystem = null;
    private GameEventSystem m_GameEventSystem = null;

    private bool m_canInput = false;
    private bool m_isGameOver = false;

    private Game() { }

    public void Initinal()
    {
        m_GameEventSystem = new GameEventSystem();
        m_StageSystem = new StageSystem();
    }

    private void ResigerGameEvent()
    {

    }

    public void Release()
    {
        m_GameEventSystem.Release();
        m_StageSystem.Release();
    }

    public void Updata()
    {
        m_GameEventSystem.Update();
        m_StageSystem.Update();
    }

    public void RegisterGameEvent(Enum.ENUM_GameEvent emGameEvent, IGameEventObserver Observer)
    {
        m_GameEventSystem.RegisterObserver(emGameEvent, Observer);
    }


    public void NotifyGameEvent(Enum.ENUM_GameEvent emGameEvent, System.Object Param)
    {
        m_GameEventSystem.NotifySubject(emGameEvent, Param);
    }


    public void SetCanInput(bool value)
    {
        m_canInput = value;
    }

    public bool GetCanInput()
    {
        return m_canInput;
    }


}
