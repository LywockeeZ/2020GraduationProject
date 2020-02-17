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
        CurrentUIType.UIForm_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Lucency;

        messageText = UITool.GetUIComponent<Text>(this.gameObject, "Txt_panel");
    }


    public void RestartBtn()
    {
        Game.Instance.CloseUI("EndStageUI");
        Game.Instance.NotifyEvent(ENUM_GameEvent.StageRestart);
    }


    public void NextBtn()
    {
        Game.Instance.CloseUI("EndStageUI");
        Game.Instance.LoadLevel("StartScene");

    }


    public override void ShowMessage(string content)
    {
        messageText.text = content;
    }
}
