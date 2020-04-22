﻿using System.Collections;
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

    public string sceneName;

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
        Game.Instance.LoadLevel(SceneManager.GetActiveScene().name);
    }

    public virtual void RestartStage()
    {
        Game.Instance.NotifyEvent(ENUM_GameEvent.StageRestart, null);
    }

    /// <summary>
    /// 加载场景，用于自由模式，不自动加载关卡
    /// </summary>
    public virtual void GoToScene()
    {
        LoadingSceneManager.LoadScene(levelName);
    }

    public virtual void GoToLevelOnMain()
    {
        Game.Instance.LoadLevelOnMain(sceneName, levelName);
    }

    public virtual void LevelWillTo()
    {
        Game.Instance.SetLevelWillToOnMain(sceneName , levelName);
    }

    public virtual void GoToNewStage()
    {
        GameTest.Instance.CurrentStage = levelName;
        Game.Instance.LoadLevel("NewStage");
    }


}
