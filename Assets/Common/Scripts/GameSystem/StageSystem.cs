using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

/// <summary>
/// 管理关卡内容，和关卡场景切换
/// Stage代表一个关卡的内容，Level为关卡内容+场景
/// </summary>
public class StageSystem : IGameSystem
{
    private Dictionary<string, IStageHandler> Stages = new Dictionary<string, IStageHandler>();
    private IStageHandler m_NowStageHandler = null;
    private IStageHandler m_NextStageHandler = null;
    private IStageHandler m_RootStageHandler = null;
    private int m_NowStageLv = 1;       //目前的关卡
    private string startStageName;
    private string levelWillToOnMain;
    private string sceneWillToOnMain;


    public StageSystem()
    {
        Initialize();
    }


    public override void Initialize()
    {
        InitializeStageData();
        RegisterEvent();
    }


    #region 事件注册与注销
    private EventListenerDelegate OnStageBegain;
    private EventListenerDelegate OnStageEnd;
    private EventListenerDelegate OnStageRestart;
    private EventListenerDelegate OnLoadSceneStart;
    private EventListenerDelegate OnLoadSceneComplete;
    private void RegisterEvent()
    {
        //关卡的开始需要一个按钮来推动
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageBegain,
        OnStageBegain = (Message evt) =>
        {
            //在此处要对加载关卡方式进行区别，用参数加载关卡必须在通知事件时带参数
            //如果参数为空，自动加载关卡链中的下一个，否则加载参数指定的关卡
            if (evt.Params == null)
            {
                LoadNextStage();
            }
            else
            {
                LoadStage(evt.Params[0].ToString());
            }
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.StageEnd,
        OnStageEnd = (Message evt) =>
        {
            DetachStageEvent();
            //关卡结束时从关卡链中获取下一个关卡
            m_NextStageHandler = m_NowStageHandler.CheckStage();
            Game.Instance.SetCanInput(false);

        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.StageRestart,
        OnStageRestart = (Message evt) =>
        {
            CoroutineManager.StopAllCoroutine();
            Game.Instance.SetCanInput(false);
            RestartStage();

        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.LoadSceneStart,
        OnLoadSceneStart = (Message evt) =>
        {
            Game.Instance.SetCanInput(false);
            Game.Instance.ResetStage();
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.LoadSceneComplete,
        OnLoadSceneComplete = (Message evt) =>
        {
            if (SceneManager.GetActiveScene().name == "StartScene")
            {
                ResetStartStage();
            }
        });
    }


    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.StageBegain, OnStageBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageEnd, OnStageEnd);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageRestart, OnStageRestart);
        Game.Instance.DetachEvent(ENUM_GameEvent.LoadSceneStart, OnLoadSceneStart);
        Game.Instance.DetachEvent(ENUM_GameEvent.LoadSceneComplete, OnLoadSceneComplete);
    }
    #endregion


    public override void Release()
    {
        DetachEvent();
    }


    public override void Update()
    {
        if (m_NowStageHandler != null)
        {
            m_NowStageHandler.Update();
        }
    }


    private void InitializeStageData()
    {
        if (m_RootStageHandler != null)
            return;

        Stages = GameFactory.GetDataFactory().LoadStageData();

        //设置起点
        SetStartStage("关卡测试1");

    }

    /// <summary>
    /// 设置起始关卡
    /// </summary>
    /// <param name="stageName"></param>
    public void SetStartStage(string stageName)
    {
        startStageName = stageName;
        m_RootStageHandler = Stages[startStageName];
        m_NextStageHandler = Stages[startStageName];
        m_NowStageHandler = null;
    }

    /// <summary>
    /// 重置关卡链为开始状态
    /// </summary>
    public void ResetStartStage()
    {
        m_RootStageHandler = Stages[startStageName];
        m_NextStageHandler = Stages[startStageName];
        m_NowStageHandler = null;
    }

    public bool IsStageEnd()
    {
        m_NextStageHandler = m_NowStageHandler.CheckStage();
        if (m_NextStageHandler == m_NowStageHandler)
            return false;
        else
            return true;
    }


    /// <summary>
    /// 清空关卡注册的事件
    /// </summary>
    public void DetachStageEvent()
    {
        NormalStageHandler stage = m_NowStageHandler as NormalStageHandler;
        stage.DetachEvent();
    }


    /// <summary>
    /// 将关卡内容清空
    /// </summary>
    public void ResetStage()
    {
        if (m_NowStageHandler != null && m_NowStageHandler.isLoaded)
        {           
            m_NowStageHandler.Reset();
        }
    }


    /// <summary>
    /// 重新开始当前关卡，只重载关卡内容，不重载场景
    /// </summary>
    public void RestartStage()
    {
        ResetStage();
        CoroutineManager.StartCoroutineTask(()=>m_NowStageHandler.Start(), 0.1f);
    }


    /// <summary>
    /// 读取并加载下一个关卡
    /// </summary>
    public void LoadNextStage()
    {
        m_NowStageHandler = m_NextStageHandler;
        m_NowStageHandler.Start();
    }


    /// <summary>
    /// 通过关卡名读取并加载指定关卡
    /// </summary>
    /// <param name="stageName"></param>
    public void LoadStage(string stageName)
    {
        if (Stages.ContainsKey(stageName))
        {
            Debug.Log("LoadStage:" + stageName);
            m_NowStageHandler = Stages[stageName];
            m_NowStageHandler.Start();
        }
        else
        {
            Debug.LogError("[" + stageName + "]此关卡不存在");
        }
    }


    /// <summary>
    /// 加载下一个关卡场景
    /// </summary>
    public void LoadNextLevel()
    {
        //如果到达关卡链尽头，自动进入开始界面
        if (m_NextStageHandler == null)
        {
            LoadingSceneManager.LoadScene("StartScene");
        }
        else
        {
            //根据关卡链中下一个关卡，通过字典获得关卡名称
            //由值获取键，因为关卡一定不会重复，所以获取第一个就可以
            var levelName = Stages.FirstOrDefault(q => q.Value == m_NextStageHandler).Key;
            LoadingSceneManager.LoadScene(levelName);
        }
    }


    /// <summary>
    /// 加载指定关卡场景
    /// </summary>
    /// <param name="levelName"></param>
    public void LoadLevel(string levelName)
    {
        //如果未传入名称则自动加载关卡链的下一个
        if (string.IsNullOrEmpty(levelName))
            LoadNextLevel();
        else
        {

            if (levelName != "NewStage")
            {
                LoadingSceneManager.LoadScene(levelName);
            }
            else
                LoadingSceneManager.LoadScene("NewStage");
        }
    }

    
    /// <summary>
    /// 切换主场景上的关卡
    /// </summary>
    /// <param name="stageName"></param>
    public void LoadLevelOnMain( string sceneName, string levelName)
    {
        levelWillToOnMain = levelName;

        //判断是否在该主场景
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            SceneChanger.Instance.FadeScene(()=> { LevelManager.Instance.LoadLevel(levelName); });
        }
        else
        {
            LoadLevel(sceneName);
        }
    }


    /// <summary>
    /// 获取当前主场景上将要加载的关卡名
    /// </summary>
    /// <returns></returns>
    public string GetLevelWillToOnMain()
    {
        return levelWillToOnMain;
    }

    public string GetSceneWillToOnMain()
    {
        return sceneWillToOnMain;
    }

    /// <summary>
    /// 设置主场景上将要加载关卡名
    /// </summary>
    /// <param name="levelName"></param>
    public void SetLevelWillToOnMain(string sceneName, string levelName)
    {
        levelWillToOnMain = levelName;
        sceneWillToOnMain = sceneName;
        //触发技能选择UI之前就设置好要加载的关卡
        if (levelWillToOnMain != null)
        {
            Stages.TryGetValue(levelWillToOnMain, out IStageHandler stage);
            if (stage != null)
            {
                m_NowStageHandler = Stages[levelWillToOnMain];
            }
        }

    }


    /// <summary>
    /// 获取关卡字典
    /// </summary>
    public Dictionary<string, IStageHandler> GetStages()
    {
        return Stages;
    }

    public IStageHandler GetCurrentStage()
    {
        return m_NowStageHandler;
    }

}
