using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject reference;
    public List<GameObject> poolInstances = new List<GameObject>();

    private void Start()
    {
    }


    private GameObject CreateInstance(Vector3 pos)
    {
        var clone = GameObject.Instantiate(reference, pos, Quaternion.identity);
        clone.name = reference.name;
        poolInstances.Add(clone);
        return clone;
    }

    /// <summary>
    /// 遍历对象池，获取可用对象
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GameObject NextObject(Vector3 pos)
    {
 
        GameObject instance = null;
        //遍历查看是否有可用的未激活的对象
        foreach (var go in poolInstances)
        {
            if (go.gameObject.activeSelf != true)
            {
                instance = go;
                instance.transform.position = pos;
            }
        }
        //如果没有就创建一个
        if (instance == null)
        {
            instance = CreateInstance(pos);
        }

        instance.SetActive(true);
        return instance; 
    }
}
