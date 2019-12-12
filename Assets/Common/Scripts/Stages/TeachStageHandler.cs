using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachStageHandler : NormalStageHandler
{
    public TeachStageHandler(NormalStageScore StageScore, NormalStageData StageData, int x, int y):base(StageScore, StageData, x, y)
    {

    }

    public override IStageHandler CheckStage()
    {
         return m_NextHandler;
    }

}
