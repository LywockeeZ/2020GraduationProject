using System.Collections;
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



    public static void StartCoroutineTask(Func<bool>Callback ,ENUM_GameEvent _GameEvent, float waitTime)
    {
        coroutine.StartCoroutine(StartInnerCoroutine(Callback, _GameEvent, waitTime));
    }



    public static void StartCoroutineTask(Action Callback, float waitTime)
    {
        coroutine.StartCoroutine(StartInnerCoroutine(Callback, waitTime));
    }



    private static IEnumerator StartInnerCoroutine(Action Callback, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Callback();
        coroutine.StopCoroutine(StartInnerCoroutine(Callback, waitTime));
    }



    private static IEnumerator StartInnerCoroutine(Func<bool>Callback, ENUM_GameEvent _GameEvent, float waitTime)
    {
        while (!Callback())
        {
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        Game.Instance.NotifyEvent(_GameEvent, null);
        coroutine.StopCoroutine(StartInnerCoroutine(Callback, _GameEvent, waitTime));
    }
}
