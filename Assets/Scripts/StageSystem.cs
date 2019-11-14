using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSystem : IGameSystem
{
    private IStageHandler m_NowStageHandler = null;
    private IStageHandler m_RootStageHandler = null;
    private int m_NowStageLv = 1;       //目前的关卡

    public StageSystem(GameManager gameManager) : base(gameManager)
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

        IStageScore StageScore = null;
        IStageHandler NewStage = null;

        //第一关
        int[,,] StageMetadata = new int[2, 4, 4] { { {1,1,1,1}, {1,1,1,1}, {1,1,1,1}, {1,1,1,1} },
                                               { {1,1,1,1}, {1,1,1,1}, {1,1,1,1}, {1,1,1,1} } };
        NormalStageData StageData = new NormalStageData(4, 4, StageMetadata);
        StageScore = new StageScoreFireControl();
        NewStage = new NormalStageHandler(StageScore, StageData);

        //设定为起始关卡
        m_RootStageHandler = NewStage;

    }
}
