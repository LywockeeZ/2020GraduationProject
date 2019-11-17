using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制火势的关卡
/// </summary>
public class FireControlStageHandler : NormalStageHandler
{
    //设置关卡分数及关卡数据
    public FireControlStageHandler(NormalStageScore StageScore, NormalStageData StageDate) : base(StageScore,StageDate)
    {
        
    }


    public override IStageHandler CheckStage()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
    }


    public override void Reset()
    {
    }

    public override void BuildStage()
    {
        throw new System.NotImplementedException();
    }

    public override BaseUnit GetBaseUnit(int x, int y)
    {
        throw new System.NotImplementedException();
    }
}
