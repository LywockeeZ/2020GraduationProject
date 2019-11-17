using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewStageSubject : IGameEventSubject
{
    public override void SetParam(object Param)
    {
        Game.Instance.SetCanInput(true);
        base.SetParam(Param);
        Notify();
    }

}
