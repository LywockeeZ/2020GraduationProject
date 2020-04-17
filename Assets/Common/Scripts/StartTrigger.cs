using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    public void Trigger()
    {
        Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, "Level1");
        Game.Instance.UIShowMessag("TestStartUI", "选择一个技能来开始关卡\n技能每个回合只能使用一次");
    }
}
