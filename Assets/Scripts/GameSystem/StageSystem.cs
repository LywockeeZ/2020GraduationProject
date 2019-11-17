using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSystem : IGameSystem
{
    private Dictionary<string, IStageHandler> Stages = new Dictionary<string, IStageHandler>();
    private IStageHandler m_NowStageHandler = null;
    private IStageHandler m_NextStageHandler = null;
    private IStageHandler m_RootStageHandler = null;
    private int m_NowStageLv = 1;       //目前的关卡

    public StageSystem()
    {
        Initialize();
    }

    public override void Initialize()
    {
        InitializeStageData();
    }

    public override void Release()
    {
        
    }

    public override void Update()
    {
        if (m_NowStageHandler != null)
        {
            m_NowStageHandler.Update();
        }
    }

    private void InitializeStageData()
    {
        if (m_RootStageHandler != null)
            return;

        IStageHandler NewStage = null;

        //第一关
        StageMetaData StageMetadata = GameFactory.GetDataFactory().LoadStageData("第1关");
        NormalStageData StageData = new NormalStageData(StageMetadata);
        NormalStageScore StageScore = new NormalStageScore();
        NewStage = new NormalStageHandler(StageScore, StageData);
        Stages.Add("第1关", NewStage);

        //设定为起始关卡
        m_RootStageHandler = NewStage;
        m_NextStageHandler = NewStage;

    }

    public bool IsStageEnd()
    {
        m_NextStageHandler = m_NowStageHandler.CheckStage();
        if (m_NextStageHandler == m_NowStageHandler)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void LoadNextStage()
    {
        if (m_NowStageHandler != null)
            m_NowStageHandler.Reset();
        m_NowStageHandler = m_NextStageHandler;
        m_NowStageHandler.Start();
    }

    public void LoadStage(string stageName)
    {
        if (Stages.ContainsKey(stageName))
        {
            if (m_NowStageHandler != null)
                m_NowStageHandler.Reset();
            Stages[stageName].Start();
        }
        else Debug.Log("关卡不存在");
    }

}
