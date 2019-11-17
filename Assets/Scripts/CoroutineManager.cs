﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CoroutineManager 
{
    private class  InnerCoroutine : MonoBehaviour { }
    private static InnerCoroutine coroutine;

    //在编译时就创建
    static CoroutineManager()
    {
        if (coroutine == null)
        {
            coroutine = new GameObject("CoroutineManager").AddComponent<InnerCoroutine>();
            GameObject.DontDestroyOnLoad(coroutine);
        }

    }

    public static void StartCoroutineTask(Func<bool>Callback ,Enum.ENUM_GameEvent _GameEvent, float waitTime)
    {
        coroutine.StartCoroutine(StartInnerCoroutine(Callback, _GameEvent, waitTime));
    }

    private static IEnumerator StartInnerCoroutine(Func<bool>Callback, Enum.ENUM_GameEvent _GameEvent, float waitTime)
    {
        while(Callback())
        {
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        Game.Instance.NotifyGameEvent(_GameEvent, null);
    }
}