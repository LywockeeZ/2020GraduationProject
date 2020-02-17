using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestStartUI : BaseUIForm
{
    public Text messageText;

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Lucency;

        messageText = UITool.GetUIComponent<Text>(this.gameObject, "Txt_panel");
    }


    public void BtnFunc()
    {
        Game.Instance.CloseUI("TestStartUI");
        Game.Instance.NotifyEvent(ENUM_GameEvent.RoundBegain);
    }

    public override void ShowMessage(string content)
    {
        messageText.text = content;
    }


}
