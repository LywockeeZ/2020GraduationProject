using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using DG.Tweening;

public class Stool : RoadBlock
{
    public override void Init()
    {
        base.Init();
        transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
    }

    public override void End()
    {
        base.End();
    }

}
