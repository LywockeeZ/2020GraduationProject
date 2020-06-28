using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEndFailureUI : StageEndSuccessUI
{
    protected override void OnEnable()
    {
        base.OnEnable();
        GetComponent<AudioSource>().time = 0.3f;
        GetComponent<AudioSource>().Play();
    }
    public override void DoNext()
    {
        if (Game.Instance.isTest)
        {
            Game.Instance.CloseAll();
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageRestart);
        }
        else
        {
            Game.Instance.CloseAll();
            LevelManager.Instance.BackToLevel();
        }
    }
}
