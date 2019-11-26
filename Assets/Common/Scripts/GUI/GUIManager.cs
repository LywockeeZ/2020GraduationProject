using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制当前场景下所有GUI效果和变化
/// </summary>
public class GUIManager : Singleton<GUIManager>
{
    public GameObject StartScreen;
    public GameObject GameScreen;
    public GameObject PauseScreen;
    public GameObject EndScreen;
    public Text APText;
    public Text CostAPText;
    public Text RoundsText;
    public Text EndText;

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual void Start()
    {
        RegisterEvent();
    }


    private EventListenerDelegate OnStageBegain;
    private EventListenerDelegate OnStageEnd;
    private EventListenerDelegate OnStageRestart;
    private EventListenerDelegate OnRoundBegain;
    private EventListenerDelegate OnRoundUpdateBegain;
    private EventListenerDelegate OnRoundEnd;
    public void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageBegain,
        OnStageBegain = (Message evt) =>
        {
            StartScreen.SetActive(true);
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.StageEnd,
        OnStageEnd = (Message evt) =>
        {
            EndScreen.SetActive(true);
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.StageRestart,
        OnStageRestart = (Message evt) =>
        {
            //重置文本框
            EndScreen.SetActive(false);
            StartScreen.SetActive(true);
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundBegain,
        OnRoundBegain = (Message evt) =>
        {
            GameScreen.SetActive(true);
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundUpdateBegain,
        OnRoundUpdateBegain = (Message evt) =>
        {
            GameScreen.SetActive(false);
        });

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundEnd,
        OnRoundEnd = (Message evt) =>
        {
            GameScreen.SetActive(false);
        });


    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.StageBegain, OnStageBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageEnd, OnStageEnd);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageRestart, OnStageRestart);
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundBegain, OnRoundBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundUpdateBegain, OnRoundUpdateBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundEnd, OnRoundEnd);
    }

    private void OnDestroy()
    {
        DetachEvent();
    }



    // Update is called once per frame
    void Update()
    {
        
    }


    #region 设置文本框的接口
    public void SetAPText(int value)
    {
        APText.text = "行动点数：" + value.ToString();
    }

    public void SetCostAPText(int value)
    {
        CostAPText.text = "花费点数：" + value.ToString();
    }

    public void SetRoundsText(int value)
    {
        RoundsText.text = "回合数：" + value.ToString();
    }

    public void SetEndText(string content)
    {
        RoundsText.text = content;
    }

    #endregion


    #region 供按钮使用的方法
    public void StartRound()
    {
        Game.Instance.NotifyEvent(ENUM_GameEvent.RoundBegain, null);
        StartScreen.SetActive(false);
    }

    public void EndRound()
    {
        Game.Instance.NotifyEvent(ENUM_GameEvent.RoundUpdateBegain, null);
    }

    #endregion
}
