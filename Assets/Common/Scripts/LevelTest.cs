using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


/// <summary>
/// 此类用来关卡场景测试
/// 当想要单独运行一个场景时，只需将此类添加到任意空物体上
/// </summary>
public class LevelTest : MonoBehaviour
{
    private bool StageStart = false;

    private void Awake()
    {
        Game.Instance.Initinal();
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!StageStart)
        {
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, SceneManager.GetActiveScene().name);
            StageStart = true;
        }
        Game.Instance.Updata();
    }







}
