using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using DG.Tweening;

public class Stone : RoadBlock
{
    private GameObject activeObj;
    public override void Init()
    {
        base.Init();
        transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        activeObj = transform.GetChild(Convert.ToInt16(UnityEngine.Random.Range(0, 3))).gameObject;
        activeObj.SetActive(true);
    }

    public override void End()
    {
        activeObj.SetActive(false);
        base.End();
    }
}
