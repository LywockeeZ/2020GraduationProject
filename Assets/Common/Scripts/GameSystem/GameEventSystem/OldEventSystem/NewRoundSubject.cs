using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoundSubject : IGameEventSubject
{
    public override void SetParam(object Param)
    {
        base.SetParam(Param);
        Game.Instance.SetCanInput(true);
        Notify();
    }

}
