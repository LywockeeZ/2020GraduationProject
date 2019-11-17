using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundUpdateEndSubject: IGameEventSubject
{
    public bool isStageEnd = false;

    public override void SetParam(object Param)
    {
        isStageEnd = Game.Instance.IsCurrentStageEnd();
        base.SetParam(Param);
        Notify();
        //若自动进行下一关卡，在这里加入else
        if (!isStageEnd)
        {
            Game.Instance.NotifyGameEvent(Enum.ENUM_GameEvent.NewRound, null);
        }
    }

}
