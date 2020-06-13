using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndStageUI : BaseUIForm
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
            Game.Instance.CloseAll();
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageRestart);
        }
        else
        {
            Game.Instance.CloseUI("BattleUI");
            Game.Instance.CloseUI("SkillBarUI");
            Game.Instance.CloseUI("EndStageUI");
            LevelManager.Instance.BackToLevel();
        }
    }


    public void NextBtn()
    {
        if (Game.Instance.isTest)
        {
            Game.Instance.CloseUI("BattleUI");
            Game.Instance.CloseUI("SkillBarUI");
            Game.Instance.CloseUI("EndStageUI");
            Game.Instance.LoadLevel("StartScene");
        }
        else
        {
            Game.Instance.CloseUI("EndStageUI");
            LevelManager.Instance.BackToLevel();
        }
    }


    public override void ShowMessage(string content)
    {
        messageText.text = content;
    }
}
