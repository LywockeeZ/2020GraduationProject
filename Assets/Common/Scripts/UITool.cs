using UnityEngine;

public static class UITool
{
    private static GameObject m_CanvasObj = null; //场景上的UI根节点

    /// <summary>
    /// 寻找限定在UIRoot节点下的UI对象
    /// </summary>
    /// <param name="UIName"></param>
    /// <returns></returns>
    public static GameObject FindUIGameObject(string UIName)
    {
        if (m_CanvasObj == null)
            m_CanvasObj = UnityTool.FindGameObject("UIRoot");
        if (m_CanvasObj == null)
            return null;
        return UnityTool.FindChildGameObject(m_CanvasObj, UIName);
    }


    /// <summary>
    /// 获取UI组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Container">UI界面</param>
    /// <param name="UIName">具体的UI部件名</param>
    /// <returns></returns>
    public static T GetUIComponent<T>(GameObject Container, string UIName) 
                where T : UnityEngine.Component
    {
        //找出子对象
        GameObject ChildGameObject = UnityTool.FindChildGameObject(Container, UIName);
        if (ChildGameObject == null)
        {
            Debug.LogWarning("未找到名为[" + UIName + "]的UI部件");
            return null;
        }

        T tempObj = ChildGameObject.GetComponent<T>();
        if (tempObj == null)
        {
            Debug.LogWarning("组件[" + UIName + "]不是[" + typeof(T) + "]");
            return null;
        }
        return tempObj;
    }
}
