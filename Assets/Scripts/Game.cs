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
    private APSystem m_APSystem = null;



    private bool m_canInput = false;
    private bool m_isGameOver = false;



    private Game() { }



    public void Initinal()
    {
        m_GameEventSystem = new GameEventSystem();
        m_StageSystem = new StageSystem();
        m_APSystem = new APSystem();
    }

    private void ResigerGameEvent()
    {

    }

    public void Release()
    {
        m_GameEventSystem.Release();
        m_StageSystem.Release();
        m_APSystem.Release();
    }

    public void Updata()
    {
        m_GameEventSystem.Update();
        m_StageSystem.Update();
        m_APSystem.Update();
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

    public void SetMaxAP(int value)
    {
        m_APSystem.ResetAPSystem();
        m_APSystem.SetRoundActionPts(value);
    }

    public int GetCurrentAP()
    {
        return m_APSystem.GetCurrentAP();
    }

    public void ResetRoundAP()
    {
        m_APSystem.ResetActionPoints();
    }

    public bool CostAP(int value, int additionValue)
    {
        return m_APSystem.CostAP(value, additionValue);
    }

    public bool IsCurrentStageEnd()
    {
        return m_StageSystem.IsStageEnd();
    }

    public void LoadNextStage()
    {
        m_StageSystem.LoadNextStage();
    }

    public void LoadStage(string stageName)
    {
        m_StageSystem.LoadStage(stageName);
    }


}
