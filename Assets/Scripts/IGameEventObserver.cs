using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏事件观察者接口，负责游戏事件触发时被通知的操作接口
/// </summary>
public abstract class IGameEventObserver
{

    public abstract void SetSubject(IGameEventSubject Subiect);
    public abstract void Update();
}
