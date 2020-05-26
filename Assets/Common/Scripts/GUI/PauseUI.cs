using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class PauseUI : BaseUIForm
{
    GameObject restartBtnObj;
    MMTouchButton restartBtn;

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Translucence;
        restartBtnObj = UnityTool.FindChildGameObject(gameObject, "Btn_restart");
        restartBtn = UITool.GetUIComponent<MMTouchButton>(restartBtnObj, "Background");
    }

    private void Update()
    {
        if (Game.Instance.GetCanFreeMove())
        {
            restartBtn.DisableButton();
        }
        else restartBtn.EnableButton();
    }

    public void ResumeBtn()
    {
        Game.Instance.CloseUI("PauseUI");
    }

    public void RestartBtn()
    {
        Game.Instance.CloseUI("PauseUI");
        Game.Instance.NotifyEvent(ENUM_GameEvent.StageRestart);
    }

    public void ReturnBtn()
    {
        Game.Instance.CloseAll();
        Game.Instance.LoadLevel("StartSceneBack");
    }


}
