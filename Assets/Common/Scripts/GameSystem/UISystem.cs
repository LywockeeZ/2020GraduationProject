using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISystem : IGameSystem
{
    public Dictionary<string, BaseUIForm> AllUIFormsCache;

    private bool isPauseUIOpen = false;

    public UISystem()
    {
        Initialize();
    }



    public override void Initialize()
    {
        AllUIFormsCache = UIManager.Instance.DicALLUIForms;
        RegisterEvent();
    }


    #region 事件的注册与撤销
    private EventListenerDelegate OnStageBegain;
    private EventListenerDelegate OnRoundBegain;
    private EventListenerDelegate OnStageEnd;
    private EventListenerDelegate OnStageRestart;
    private EventListenerDelegate OnGamePause;
    private EventListenerDelegate OnLoadSceneStart;
    private EventListenerDelegate OnLoadSceneComplete;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.GamePause,
            OnGamePause = (Message evt) =>
            {
                ShowUI("PauseUI");
            });

        Game.Instance.RegisterEvent(ENUM_GameEvent.StageBegain,
            OnStageBegain = (Message evt) =>
            {
                if (Game.Instance.isTest)
                {
                    //Game.Instance.ShowUI("TestStartUI");
                    Game.Instance.ShowUI("SkillSelectUI");
                }
                else Game.Instance.NotifyEvent(ENUM_GameEvent.RoundBegain);

            });

        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundBegain,
            OnRoundBegain = (Message evt) =>
            {
                if (!Game.Instance.isTest)
                {
                    void action()
                    {
                        Game.Instance.ShowUI("BattleUI");
                    }
                    CoroutineManager.StartCoroutineTask(action, 1.2f);
                }
                else Game.Instance.ShowUI("BattleUI");
            });

        Game.Instance.RegisterEvent(ENUM_GameEvent.StageEnd,
            OnStageEnd = (Message evt) =>
            {
                AudioManager.Instance.SoundsStop();
                Game.Instance.CloseAll();
                void action()
                {
                    if (evt != null && evt.Params.Length != 0)
                    {
                        switch (evt.Params[0])
                        {
                            case 0:
                                Game.Instance.ShowUI("StageEndSuccessUI");
                                //string content = "花费点数：" + Game.Instance.GetTotalCostPts().ToString();
                                //Game.Instance.UIShowMessag("EndSuccessUI", content);
                                break;
                            case 1:
                                Game.Instance.ShowUI("StageEndFailureUI");
                                //Game.Instance.UIShowMessag("EndStageUI", "你失败了！");
                                break;
                            case 2:
                                Game.Instance.ShowUI("StageEndFailureUI");
                                //Game.Instance.UIShowMessag("EndStageUI", "很遗憾，幸存者被烧死了！");
                                break;
                            case 4:
                                Game.Instance.ShowUI("StageEndFailureUI");
                                //string content2 = "花费点数：" + Game.Instance.GetTotalCostPts().ToString();
                                //Game.Instance.UIShowMessag("EndStageUI", content2);
                                break;
                            default:
                                break;
                        }
                    }
                }
                CoroutineManager.StartCoroutineTask(action, 0.5f);
                //Game.Instance.ShowUI("EndStageUI");
            });

        Game.Instance.RegisterEvent(ENUM_GameEvent.StageRestart,
            OnStageRestart = (Message evt) =>
            {
                void action()
                {
                    Game.Instance.ShowUI("SkillSelectUI");
                }
                CoroutineManager.StartCoroutineTask(action, 0.25f);
            });

        Game.Instance.RegisterEvent(ENUM_GameEvent.LoadSceneStart,
            OnLoadSceneStart = (Message evt) =>
            {
                Game.Instance.CloseAll();
            });


    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.StageBegain, OnStageBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundBegain, OnRoundBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageEnd, OnStageEnd);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageRestart, OnStageRestart);
        Game.Instance.DetachEvent(ENUM_GameEvent.GamePause, OnGamePause);
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
        if (SceneManager.GetActiveScene().name != "StartScene" && 
            SceneManager.GetActiveScene().name != "StartSceneBack" &&
            SceneManager.GetActiveScene().name != "LevelSelector")
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                if (!isPauseUIOpen)
                {
                    Game.Instance.SetCanInput(false);
                    ShowUI("PauseUI");
                    isPauseUIOpen = true;
                }
                else
                {
                    Game.Instance.SetCanInput(true);
                    CloseUI("PauseUI");
                    isPauseUIOpen = false;
                }
            }
        }
    }


    /// <summary>
    /// 打开一个UI界面
    /// </summary>
    /// <param name="UIFormName"></param>
    public void ShowUI(string UIFormName)
    {
        Debug.Log("ShowUI:" + UIFormName);
        UIManager.Instance.ShowUIForms(UIFormName);
    }


    /// <summary>
    /// 关闭一个UI界面
    /// </summary>
    /// <param name="UIFormName"></param>
    public void CloseUI(string UIFormName)
    {
        Debug.Log("CloseUI:" + UIFormName);
        UIManager.Instance.CloseOrReturnUIForms(UIFormName);
    }


    public void CloseAll()
    {
        Debug.Log("CloseAllUI");
        UIManager.Instance.CloseAll();
    }


    public void TriggerPopUp(string content)
    {
        UIManager.Instance.PopUp(content);
    }


    #region 设置UI的接口

    /// <summary>
    /// 设置BattleUI中ActionBar的UI接口
    /// </summary>
    /// <param name="ap"></param>
    /// <param name="round"></param>
    public void SetActionBar(int ap)
    {
        BaseUIForm UIForm = null;
        BattleUI battleUI = null;

        AllUIFormsCache.TryGetValue("BattleUI", out UIForm);
        if (UIForm == null)
            Debug.LogWarning("界面[BattleUI]未在缓存中");
        else
        {
            battleUI = UIForm as BattleUI;
            battleUI.SetActionBar(ap);
        }
        
    }



    /// <summary>
    /// 设置BattleUI中RoungTag的接口
    /// </summary>
    /// <param name="ap"></param>
    /// <param name="round"></param>
    public void SetRoundTag(int round)
    {
        BaseUIForm UIForm = null;
        BattleUI battleUI = null;

        AllUIFormsCache.TryGetValue("BattleUI", out UIForm);
        if (UIForm == null)
            Debug.LogWarning("界面[BattleUI]未在缓存中");
        else
        {
            battleUI = UIForm as BattleUI;
            battleUI.SetRoundTag(round);
        }

    }


    public void ShowMessage(string UIFormName, string content)
    {
        BaseUIForm UIForm = null;

        AllUIFormsCache.TryGetValue(UIFormName, out UIForm);
        if (UIForm == null)
            Debug.LogWarning("界面[" + UIFormName + "]未在缓存中");
        else
        {
            UIForm.ShowMessage(content);
        }

    }

    #endregion

}
