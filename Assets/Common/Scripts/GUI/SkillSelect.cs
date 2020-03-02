using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class SkillSelect : MonoBehaviour
{
    MMTouchButton Button;
    void Start()
    {
        Button = gameObject.GetComponent<MMTouchButton>();
        //RegisterEvent();
    }

    public void SelectSkill()
    {
        //Game.Instance.GetPlayerUnit().ExecuteSkill();
        //Button.DisableButton();
    }

    private EventListenerDelegate OnRoundBegain;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundBegain,
            OnRoundBegain = (Message evt) =>
            {
                Button.EnableButton();
            });
    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundBegain, OnRoundBegain);
    }

    private void OnDestroy()
    {
        //DetachEvent();
    }

}
