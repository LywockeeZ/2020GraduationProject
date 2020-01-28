using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAssetFactory : IAssetFactory
{
    //模型所在资源文件夹下的路径
    public const string ModelPrefabsPath = "Prefabs/";
    public const string UIPath = "UI/";


    public override GameObject LoadModel(string AssetName, Vector3 Position)
    {
        return InstantiateGameObject(ModelPrefabsPath + AssetName, Position);
    }

    public override GameObject LoadUI(string UIName)
    {
        return InstantiateGameObject(UIPath + UIName, Vector3.zero);
    }



    // 产生GameObject
    private GameObject InstantiateGameObject(string AssetName, Vector3 Position)
    {
        // 从Resrouce中载入
        UnityEngine.Object res = LoadGameObjectFromResourcePath(AssetName);
        if (res == null)
            return null;
        return UnityEngine.Object.Instantiate(res, Position, Quaternion.identity) as GameObject;
    }



    // 从Resrouce中载入
    public UnityEngine.Object LoadGameObjectFromResourcePath(string AssetPath)
    {
        UnityEngine.Object res = Resources.Load(AssetPath);
        if (res == null)
        {
            Debug.LogWarning("无法载入路径[" + AssetPath + "]上的Asset");
            return null;
        }
        return res;
    }


}
