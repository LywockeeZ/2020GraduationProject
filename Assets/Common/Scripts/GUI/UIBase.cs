using UnityEngine;
using System.Collections;

//GameObject Loaded CallBack 物体加载回掉
public delegate void GameObjectLoadedCallBack(GameObject obj);

/// <summary>
/// UI层级
/// </summary>
public enum UILayerType
{
    Top,
    Upper,
    Normal,
    Hud
}

/// <summary>
/// 加载UI的方式
/// </summary>
public enum UILoadType
{
    SyncLoad,
    AsyncLoad,
}

public abstract class UIBase
{
    /// <summary>
    /// 是否加载完成的标志位
    /// </summary>
    protected bool m_isInited;

    /// <summary>
    /// UI名字
    /// </summary>
    protected string m_uiName;

    /// <summary>
    /// 在关闭的时候是否缓存UI 默认不缓存
    /// </summary>
    protected bool m_isCatchUI = false;

    /// <summary>
    /// UI的实例化GamObejct
    /// </summary>
    protected GameObject m_uiGameObject;

    /// <summary>
    /// 设置UI可见性状态
    /// </summary>
    protected bool m_active = false;

    /// <summary>
    /// 加载完成的回调
    /// </summary>
    protected GameObjectLoadedCallBack m_callBack;

    /// <summary>
    /// UI的资源全路径
    /// </summary>
    protected string m_uiFullPath = "UI/";

    //UILayerType UI层类型
    protected UILayerType m_uiLayerType;

    //UI的加载方式
    protected UILoadType m_uiLoadType;

    public string UIName
    {
        get { return m_uiName; }
        set
        {
            m_uiName = value;//.EndsWith(MPathUtils.UI_PREFAB_SUFFIX) ? m_uiName = value : m_uiName = value + MPathUtils.UI_PREFAB_SUFFIX;
            m_uiFullPath = "UI/" + m_uiName;//MPathUtils.UI_MAINPATH + "/" + m_uiName;
        }
    }

    public bool IsCatchUI
    {
        get { return m_isCatchUI; }
        set
        {
            m_isCatchUI = value;
        }
    }

    public GameObject UIGameObject
    {
        get { return m_uiGameObject; }
        set { m_uiGameObject = value; }
    }

    public bool Active
    {
        get { return m_active; }
        set
        {
            m_active = value;
            if (m_uiGameObject != null)
            {
                m_uiGameObject.SetActive(value);
                if (m_uiGameObject.activeSelf)
                {
                    OnActive();
                }
                else
                {
                    OnDeActive();
                }
            }
        }
    }

    public bool IsInited { get { return m_isInited; } }

    protected UIBase(string uiName, UILayerType layerType, UILoadType loadType = UILoadType.SyncLoad)
    {
        UIName = uiName;
        m_uiLayerType = layerType;
        m_uiLoadType = loadType;
    }

    public virtual void Init()
    {
        if (m_uiLoadType == UILoadType.SyncLoad)
        {
            GameObject go = GameFactory.GetAssetFactory().LoadUI(m_uiName);//MObjectManager.singleton.InstantiateGameObeject(m_uiFullPath);
            OnGameObjectLoaded(go);
        }
        else
        {
            //MObjectManager.singleton.InstantiateGameObejectAsync(m_uiFullPath, delegate (string resPath, MResourceObjectItem mResourceObjectItem, object[] parms)
            //{
            //    GameObject go = mResourceObjectItem.m_cloneObeject;
            //    OnGameObjectLoaded(go);
            //}, LoadResPriority.RES_LOAD_LEVEL_HEIGHT);
        }
    }

    private void OnGameObjectLoaded(GameObject uiObj)
    {
        if (uiObj == null)
        {
            Debug.LogError("UI加载失败了老铁~看看路径 ResPath: " + m_uiFullPath);
            return;
        }
        m_uiGameObject = uiObj;
        m_isInited = true;
        SetPanetByLayerType(m_uiLayerType);
        m_uiGameObject.transform.localPosition = Vector3.zero;
        m_uiGameObject.transform.localScale = Vector3.one;
    }

    public virtual void Uninit()
    {
        m_isInited = false;
        m_active = false;
        if (m_isCatchUI)
        {
            //资源并加入到资源池
            //MObjectManager.singleton.ReleaseObject(m_uiGameObject);
            m_uiGameObject.SetActive(false);
        }
        else
        {
            //彻底清除Object资源
            //MObjectManager.singleton.ReleaseObject(m_uiGameObject, 0, true);
            m_uiGameObject.SetActive(false);
        }
    }

    protected abstract void OnActive();

    protected abstract void OnDeActive();

    public virtual void Update(float deltaTime)
    {

    }

    public virtual void LateUpdate(float deltaTime)
    {

    }

    public virtual void OnLogOut()
    {

    }

    protected void SetPanetByLayerType(UILayerType layerType)
    {
        switch (m_uiLayerType)
        {
            case UILayerType.Top:
                m_uiGameObject.transform.SetParent(UIManager.Instance.MTransTop);
                break;
            case UILayerType.Upper:
                m_uiGameObject.transform.SetParent(UIManager.Instance.MTransUpper);
                break;
            case UILayerType.Normal:
                m_uiGameObject.transform.SetParent(UIManager.Instance.MTransNormal);
                break;
            case UILayerType.Hud:
                m_uiGameObject.transform.SetParent(UIManager.Instance.MTransHUD);
                break;
        }
    }
}