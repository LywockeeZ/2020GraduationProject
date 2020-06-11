using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSuccessUI : BaseUIForm
{
    public Text messageText;

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.Normal;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.HideOther;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Lucency;

        messageText = UITool.GetUIComponent<Text>(this.gameObject, "Txt_panel");
    }


    public void RestartBtn()
    {
        if (Game.Instance.isTest)
        {
            Game.Instance.CloseUI("EndSuccessUI");
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageRestart);
        }
        else
        {
            Game.Instance.CloseUI("EndSuccessUI");
            LevelManager.Instance.LevelToNext();
        }
    }


    public void NextBtn()
    {
        if (Game.Instance.isTest)
        {
            Game.Instance.CloseUI("EndSuccessUI");
            Game.Instance.LoadLevel("LevelSelector");
        }
        else
        {
            Game.Instance.CloseUI("EndSuccessUI");
            LevelManager.Instance.LevelToNext();
        }
    }


    public override void ShowMessage(string content)
    {
        messageText.text = content;
    }
}
