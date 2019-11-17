using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundUpdateBegainSubject : IGameEventSubject
{
    public override void SetParam(object Param)
    {
        base.SetParam(Param);
        Game.Instance.SetCanInput(false);
        Notify();
    }
}
