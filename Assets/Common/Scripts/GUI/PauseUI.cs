using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : BaseUIForm
{
    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Translucence;
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
        Game.Instance.LoadLevel("StartScene");
    }


}
