using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


/// <summary>
/// 此类用来关卡场景测试
/// 承担游戏系统的启动入口
/// </summary>
public class GameTest : Singleton<GameTest>
{
    /// <summary>
    /// 用来给LevelTest传递需要加载的关卡名
    /// 由选择界面转入的关卡会将关卡名传递给它
    /// 由场景直接启动的关卡直接获取关卡名
    /// </summary>
    public string CurrentStage;

    protected override void Awake()
    {
        base.Awake();
        //区分
        if (SceneManager.GetActiveScene().name != "NewStage")
        {
            CurrentStage = SceneManager.GetActiveScene().name;
        }

        Game.Instance.Initinal();
        DontDestroyOnLoad(gameObject);
    }


    void Update()
    {
        Game.Instance.Updata();

    }







}
