using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSpawnPoint : MonoBehaviour
{
    /// <summary>
    /// 存放关卡生成点的字典
    /// </summary>
    public static Dictionary<string, GameObject> SpawnPoint = new Dictionary<string, GameObject>();

    /// <summary>
    /// 将要在此处加载的关卡
    /// </summary>
    public string StageToLoad;

    private void OnEnable()
    {
        SpawnPoint.Add(StageToLoad, gameObject);
    }

    private void OnDisable()
    {
        SpawnPoint.Remove(StageToLoad);
    }

}
