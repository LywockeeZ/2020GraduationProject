using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// 主要用于UI界面，跳转到指定关卡
/// </summary>
public class LevelSelector : MonoBehaviour
{
    //目标关卡
    public string levelName;

    /// <summary>
    /// 使用Loading界面跳转到inspector面板上输入的关卡
    /// </summary>
    public virtual void GoToLevel()
    {
        Game.Instance.LoadLevel(levelName);
    }

    /// <summary>
    /// 重新开始当前关卡
    /// </summary>
    public virtual void RestartLevel()
    {
        //Game.Instance.LoadLevel(SceneManager.GetActiveScene().name);
        Game.Instance.NotifyEvent(ENUM_GameEvent.StageRestart, null);
    }

    public virtual void StartNextStage()
    {
        Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain,null);
    }

}
