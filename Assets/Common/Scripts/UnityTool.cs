using UnityEngine;

public static class UnityTool
{
    /// <summary>
    /// 找到场景上的游戏对象
    /// </summary>
    /// <param name="GameObjectName"></param>
    /// <returns></returns>
    public static GameObject FindGameObject(string GameObjectName)
    {
        //找出对应的GameObject
        GameObject pTmpGameObj = GameObject.Find(GameObjectName);
        if (pTmpGameObj == null)
        {

            pTmpGameObj = GameObject.Find(GameObjectName + "(Clone)");
            if (pTmpGameObj == null)
            {
                Debug.LogWarning("场景中找不到GameObject[" + GameObjectName + "]对象");

                return null;

            }
        }
        return pTmpGameObj;
    }


    /// <summary>
    /// 获取某个对象下的子对象
    /// </summary>
    /// <param name="Container"></param>
    /// <param name="gameobjectName"></param>
    /// <returns></returns>
    public static GameObject FindChildGameObject(GameObject Container, string gameobjectName)
    {
        if (Container == null)
        {
            Debug.LogError("NGUICustomTools.GetChild:Container = null");
            return null;
        }

        Transform pGameObjectTF = null;

        if (Container.name == gameobjectName)
            pGameObjectTF = Container.transform;
        else
        {
            //找出所有子组件
            Transform[] allChildren = Container.transform.GetComponentsInChildren<Transform>();

            foreach (Transform child in allChildren)
            {
                if (child.name == gameobjectName)
                {
                    if (pGameObjectTF == null)
                        pGameObjectTF = child;
                    else
                        Debug.LogWarning("Container[" + Container.name 
                            + "]下找出重复组件[" + gameobjectName + "]");                                           
                    
                }
            }
        }

        // 都没找到
        if (pGameObjectTF == null)
        {
            Debug.LogError("组件[" + Container.name + "]找不到子组件["
                + gameobjectName + "]");
            return null;
        }
        return pGameObjectTF.gameObject;

    }
}
