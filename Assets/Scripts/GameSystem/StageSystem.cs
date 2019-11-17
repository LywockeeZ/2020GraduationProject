using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSystem : IGameSystem
{
    private IStageHandler m_NowStageHandler = null;
    private IStageHandler m_RootStageHandler = null;
    private int m_NowStageLv = 1;       //目前的关卡

    public StageSystem()
    {
        Initialize();
    }

    public override void Initialize()
    {
        
    }

    public override void Release()
    {
        
    }

    public override void Update()
    {
        
    }

    private void InitializeStageData()
    {
        if (m_RootStageHandler != null)
            return;

        IStageHandler NewStage = null;

        //第一关
        StageMetaData StageMetadata = GameFactory.GetDataFactory().LoadStageData("第1关");
        NormalStageData StageData = new NormalStageData(StageMetadata);
        NormalStageScore StageScore = new StageScoreFireControl();
        NewStage = new NormalStageHandler(StageScore, StageData);

        //设定为起始关卡
        m_RootStageHandler = NewStage;

    }
}
