using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAssetFactory
{
    /// <summary>
    /// 通过对象池将某预制体实例化到某位置
    /// </summary>
    /// <param name="PrefabName"></param>
    /// <param name="Position"></param>
    /// <returns></returns>
    public abstract GameObject InstantiateGameObject(string PrefabName, Vector3 Position);

    /// <summary>
    /// 通过路径将某类型的资源实例化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="AssetPath"></param>
    /// <param name="Position"></param>
    /// <returns></returns>
    public abstract T InstantiateGameObject<T>(string AssetPath, Vector3 Position) where T : UnityEngine.Object;

    /// <summary>
    /// 通过对象池回收对象
    /// 普通游戏对象：T为GameObject  普通游戏对象设置为false，并设置对象池为父物体
    /// 上层游戏单元：T为IUpperUnit  上层单元回收细节由End方法决定，并设置对象池为父物体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    public abstract void DestroyGameObject<T>(GameObject gameObject) where T : class;

    /// <summary>
    /// 通过路径加载某类型资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="AssetPath"></param>
    /// <returns></returns>
    public abstract T LoadAsset<T>(string AssetPath) where T : UnityEngine.Object;
}
