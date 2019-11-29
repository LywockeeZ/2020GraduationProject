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


    public static void StartCoroutine(IEnumerator routine)
    {
        coroutine.StartCoroutine(routine);
    }


    public static void StopCoroutine(IEnumerator routine)
    {
        coroutine.StopCoroutine(routine);
    }


    /// <summary>
    /// 使用一个返回bool类型的回调函数,
    /// 当返回为true时，等待一定时间触发事件
    /// </summary>
    /// <param name="Callback"></param>
    /// <param name="_GameEvent"></param>
    /// <param name="waitTime"></param>
    public static void StartCoroutineTask(Func<bool>Callback ,ENUM_GameEvent _GameEvent, float waitTime)
    {
        coroutine.StartCoroutine(StartInnerCoroutine(Callback, _GameEvent, waitTime));
    }


    /// <summary>
    /// 等待一定时间后执行任务
    /// </summary>
    /// <param name="Callback"></param>
    /// <param name="waitTime"></param>
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
