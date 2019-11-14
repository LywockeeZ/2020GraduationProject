using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责统计关卡分数，判定关卡是否结束
/// </summary>
public abstract class IStageScore 
{
    public IStageHandler m_StageHandler = null;

    public abstract bool CheckScore();

    public void SetStageHandler(IStageHandler stageHandler)
    {
        m_StageHandler = stageHandler;
    }

}
