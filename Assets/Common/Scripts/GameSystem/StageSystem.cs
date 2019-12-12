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



    public StageSystem()
    {
        Initialize();
    }



    public override void Initialize()
    {
        InitializeStageData();
        RegisterEvent();
    }


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
            Game.Instance.SetCanFreeMove(false);
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
            RestartStage();
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.LoadSceneStart,
        OnLoadSceneStart = (Message evt) =>
        {
            ResetStage();
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.LoadSceneComplete,
        OnLoadSceneComplete = (Message evt) =>
        {
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, null);
        });




    }





    public override void Release()
    {
        
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


        ////第1关
        //StageMetaData StageMetadata = GameFactory.GetDataFactory().LoadStageData("第1关");
        //NormalStageData StageData = new NormalStageData(StageMetadata);
        //NormalStageScore StageScore = new NormalStageScore();
        //NormalStageHandler NewStage = new NormalStageHandler(StageScore, StageData);
        //Stages.Add("第1关", NewStage);

        ////设定为起始关卡
        //m_RootStageHandler = NewStage;
        //m_NextStageHandler = NewStage;


        ////第2关
        //StageMetaData StageMetadata2 = GameFactory.GetDataFactory().LoadStageData("第2关");
        //NormalStageData StageData2 = new NormalStageData(StageMetadata2);
        //NormalStageScore StageScore2 = new NormalStageScore();
        //NormalStageHandler NewStage2 = NewStage.SetNextHandler(new NormalStageHandler(StageScore2, StageData2)) as NormalStageHandler;
        //Stages.Add("第2关", NewStage2);

        ////设置关卡链的尽头
        //NormalStageHandler NewStage3 = NewStage2.SetNextHandler(null) as NormalStageHandler;

        ////FireTest
        //StageMetaData FireTestMetadata = GameFactory.GetDataFactory().LoadStageData("FireTest");
        //NormalStageData FireTestData = new NormalStageData(FireTestMetadata);
        //NormalStageScore FireTestScore = new NormalStageScore();
        //NormalStageHandler FireTest = new NormalStageHandler(FireTestScore, FireTestData);
        //Stages.Add("FireTest", FireTest);

        ////OilTest
        //StageMetaData OilTestMetadata = GameFactory.GetDataFactory().LoadStageData("OilTest");
        //NormalStageData OilTestData = new NormalStageData(OilTestMetadata);
        //NormalStageScore OilTestScore = new NormalStageScore();
        //NormalStageHandler OilTest = new NormalStageHandler(OilTestScore, OilTestData);
        //Stages.Add("OilTest", OilTest);

        ////WaterTest
        //StageMetaData WaterTestMetadata = GameFactory.GetDataFactory().LoadStageData("WaterTest");
        //NormalStageData WaterTestData = new NormalStageData(WaterTestMetadata);
        //NormalStageScore WaterTestScore = new NormalStageScore();
        //NormalStageHandler WaterTest = new NormalStageHandler(WaterTestScore, WaterTestData);
        //Stages.Add("WaterTest", WaterTest);

        //////OilTankTest
        ////StageMetaData OilTankTestMetadata = GameFactory.GetDataFactory().LoadStageData("OilTankTest");
        ////NormalStageData OilTankTestData = new NormalStageData(OilTankTestMetadata);
        ////NormalStageScore OilTankTestScore = new NormalStageScore();
        ////NormalStageHandler OilTankTest = new NormalStageHandler(OilTankTestScore, OilTankTestData);
        ////Stages.Add("OilTankTest", OilTankTest);

        StageMetaData StageMetadata1 = GameFactory.GetDataFactory().LoadStageData("教程1");
        NormalStageData StageData1 = new NormalStageData(StageMetadata1);
        TeachStageScore StageScore1 = new TeachStageScore();
        TeachStageHandler stage1 = new TeachStageHandler(StageScore1, StageData1, 11, 5);
        Stages.Add("教程1", stage1);

        StageMetaData StageMetadata2 = GameFactory.GetDataFactory().LoadStageData("教程2");
        NormalStageData StageData2 = new NormalStageData(StageMetadata2);
        TeachStageScore StageScore2 = new TeachStageScore();
        TeachStageHandler stage2 = stage1.SetNextHandler( new TeachStageHandler(StageScore2, StageData2, 10, 0)) as TeachStageHandler;
        Stages.Add("教程2", stage2);

        StageMetaData StageMetadata3 = GameFactory.GetDataFactory().LoadStageData("教程3");
        NormalStageData StageData3 = new NormalStageData(StageMetadata3);
        TeachStageScore StageScore3 = new TeachStageScore();
        TeachStageHandler stage3 = stage2.SetNextHandler(new TeachStageHandler(StageScore3, StageData3, 0, 0)) as TeachStageHandler;
        Stages.Add("教程3", stage3);

        //设置起点
        m_RootStageHandler = stage1;
        m_NextStageHandler = stage1;
        //设置终点为空
        TeachStageHandler stage4 = stage3.SetNextHandler(null) as TeachStageHandler;

    }


    /// <summary>
    /// 临时从文件中读取并加载目标关卡数据
    /// </summary>
    /// <param name="stageName"></param>
    /// <returns></returns>
    private NormalStageHandler AutoFindAndLoadStageData(string stageName,int x, int y)
    {
        StageMetaData StageMetadata = GameFactory.GetDataFactory().LoadStageData(stageName);
        NormalStageData StageData = new NormalStageData(StageMetadata);
        NormalStageScore StageScore = new NormalStageScore();
        NormalStageHandler NewStage = new NormalStageHandler(StageScore, StageData, x, y);
        return NewStage;
    }

    private NormalStageHandler AutoFindAndLoadTeachStageData(string stageName, int x, int y)
    {
        StageMetaData StageMetadata = GameFactory.GetDataFactory().LoadStageData(stageName);
        NormalStageData StageData = new NormalStageData(StageMetadata);
        TeachStageScore StageScore = new TeachStageScore();
        NormalStageHandler NewStage = new NormalStageHandler(StageScore, StageData, x, y);
        return NewStage;
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
        m_NowStageHandler.Reset();
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
            m_NowStageHandler = Stages[stageName];
            m_NowStageHandler.Start();
        }
        else
        {
            m_NowStageHandler = AutoFindAndLoadStageData(stageName, 0, 0);
            m_NowStageHandler.Start();
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
            Game.Instance.NotifyEvent(ENUM_GameEvent.LoadSceneStart);
            LoadingSceneManager.LoadScene("StartScene");
        }
        else
        {
            //根据关卡链中下一个关卡，通过字典获得关卡名称
            //由值获取键，因为关卡一定不会重复，所以获取第一个就可以
            var levelName = Stages.FirstOrDefault(q => q.Value == m_NextStageHandler).Key;
            Game.Instance.NotifyEvent(ENUM_GameEvent.LoadSceneStart);
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
            Game.Instance.NotifyEvent(ENUM_GameEvent.LoadSceneStart, levelName);
            LoadingSceneManager.LoadScene(levelName);
        }
    }

}
