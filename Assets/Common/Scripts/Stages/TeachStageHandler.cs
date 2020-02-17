using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachStageHandler : NormalStageHandler
{
    public TeachStageHandler(TeachStageScore StageScore, NormalStageData StageData):base(StageScore, StageData)
    {

    }

    public override IStageHandler CheckStage()
    {
         return m_NextHandler;
    }

}
