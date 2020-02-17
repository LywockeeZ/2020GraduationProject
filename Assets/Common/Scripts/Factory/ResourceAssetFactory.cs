using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAssetFactory : IAssetFactory
{
    //模型所在资源文件夹下的路径
    public const string ModelPrefabsPath = "Prefabs/";
    //对象池的字典
    public Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
    //对象池的父节点
    private GameObject objectPool;

    public ResourceAssetFactory()
    {
        objectPool = new GameObject("ObjectPools");
        GameObject.DontDestroyOnLoad(objectPool);
    }


    /// <summary>
    /// 通过对象池将某预制体实例化到某位置
    /// </summary>
    /// <param name="PrefabName"></param>
    /// <param name="Position"></param>
    /// <returns></returns>
    public override GameObject InstantiateGameObject(string PrefabName, Vector3 Position)
    {
        GameObject instance = null;

        var pool = GetObjectPool(PrefabName);
        instance = pool.NextObject(Position).gameObject;

        return instance;
    }


    /// <summary>
    /// 通过路径将某类型的资源实例化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="AssetPath"></param>
    /// <param name="Position"></param>
    /// <returns></returns>
    public override T InstantiateGameObject<T>(string AssetPath, Vector3 Position)
    {
        T res = LoadAsset<T>(AssetPath);
        return GameObject.Instantiate(res, Position, Quaternion.identity);
    }


    /// <summary>
    /// 通过对象池回收对象
    /// 普通游戏对象：T为GameObject  普通游戏对象设置为false，并设置对象池为父物体
    /// 上层游戏单元：T为IUpperUnit  上层单元回收细节由End方法决定，并设置对象池为父物体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    public override void DestroyGameObject<T>(GameObject gameObject)
    {
        if (typeof(T) == typeof(IUpperUnit))
        {
            gameObject.GetComponent<IUpperUnit>().End();
        }
        else
            gameObject.SetActive(false);

        //将对象移动到对象池的子物体
        ObjectPool pool;
        pools.TryGetValue(gameObject.name , out pool);

        if (pool != null)
        {
            gameObject.transform.SetParent(pools[gameObject.name].transform);
        }
    }



    /// <summary>
    /// 载入Resource目录下的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="AssetPath"></param>
    /// <returns></returns>
    public override T LoadAsset<T>(string AssetPath)
    {
        T res = Resources.Load<T>(AssetPath);
        if (res == null)
        {
            Debug.LogError("无法载入路径[" + AssetPath + "]上的Asset");
            return default;
        }
        return res;
    }



    private ObjectPool GetObjectPool(string prefabName)
    {
        ObjectPool pool = null;
        if (pools.ContainsKey(prefabName))
        {
            pool = pools[prefabName];
        }
        else
        {

            var poolContainer = new GameObject(prefabName + "ObjectPool");
            poolContainer.transform.SetParent(objectPool.transform);
            pool = poolContainer.AddComponent<ObjectPool>();
            pools.Add(prefabName, pool);

            //从Resrouce中载入
            GameObject res = LoadAsset<GameObject>(ModelPrefabsPath + prefabName);
            if (res == null)
            return null;

            pool.reference = res;
        }
        return pool;
    }

}
