using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggersLevel1 : MonoBehaviour
{
    public void Trigger1()
    {
        Game.Instance.SetCanInput(true);
        Game.Instance.SetCanFreeMove(false);
        UIManager.Instance.ShowUIForms("BattleUI");
        Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, null);
    }
}
