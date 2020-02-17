using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责统计关卡分数，判定关卡是否结束
/// </summary>
public abstract class IStageScore 
{
    /// <summary>
    /// 判定关卡结束的方法
    /// </summary>
    /// <returns></returns>
    public abstract bool CheckStage();

    /// <summary>
    /// 判断回合是否需要更新
    /// </summary>
    /// <returns></returns>
    public abstract void CheckRound();

}
