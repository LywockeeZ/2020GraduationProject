/*
 * Description: 
 *      功能：整个UI框架的核心，用户程序通过调用本类，来调用本框架的大多数功能。  
 *      功能1：关于入“栈”与出“栈”的UI窗体4个状态的定义逻辑
 *            入栈状态：
 *                      Freeze();   （上一个UI窗体）冻结
 *                      Display();  （本UI窗体）显示
 *            出栈状态： 
 *                      Hiding(); (本UI窗体) 隐藏
 *                      Redisplay(); (上一个UI窗体) 重新显示
 *      功能2：增加“非栈”缓存集合。 
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    //缓存所有UI窗体的预制体(参数1：窗体预制体名称，2：表示对应的预制体)
    public Dictionary<string, BaseUIForm> DicALLUIForms;
    //UI窗体预制体路径(参数1：窗体预制体名称，2：表示窗体预制体路径)
    private Dictionary<string, string> _DicFormsPaths;
    //“当前UI窗体”集合。
    private Stack<BaseUIForm> _StaCurrentUIForms;
    //当前显示的UI窗体
    private Dictionary<string, BaseUIForm> _DicCurrentShowUIForms;
    //UI根节点
    private Transform _TraCanvasTransfrom = null;
    //全屏幕显示的节点
    private Transform _TraNormal = null;
    //固定显示的节点
    private Transform _TraFixed = null;
    //弹出节点
    private Transform _TraPopUp = null;
    //UI管理脚本的节点
    private Transform _TraUIScripts = null;



    //初始化核心数据，加载“UI窗体路径”到集合中。
    protected override void Awake()
    {
        base.Awake();
        //字段初始化
        DicALLUIForms = new Dictionary<string, BaseUIForm>();
        _DicCurrentShowUIForms = new Dictionary<string, BaseUIForm>();
        _DicFormsPaths = new Dictionary<string, string>();
        _StaCurrentUIForms = new Stack<BaseUIForm>();

        //初始化加载（根UI窗体）Canvas预设
        InitRootCanvasLoading();

        //得到UI根节点、全屏节点、固定节点、弹出节点
        _TraCanvasTransfrom = UnityTool.FindGameObject("UIRoot").transform;
        _TraNormal = _TraCanvasTransfrom.Find("Normal");
        _TraFixed = _TraCanvasTransfrom.Find("Fixed");
        _TraPopUp = _TraCanvasTransfrom.Find("PopUp");
        _TraUIScripts = _TraCanvasTransfrom.Find("ScriptManager");

        //把本脚本作为“根UI窗体”的子节点。
        this.gameObject.transform.SetParent(_TraUIScripts, false);
        this.gameObject.name = "UIManager";

        //"根UI窗体"在场景转换的时候，不允许销毁
        DontDestroyOnLoad(_TraCanvasTransfrom);

        //初始化“UI窗体预设”路径数据
        //先写简单的，后面使用Json做配置文件，来完善。
        if (_DicFormsPaths != null)
        {
            _DicFormsPaths.Add("PopUpUI", @"UI\PopUpUI");
            _DicFormsPaths.Add("BattleUI", @"UI\BattleUI");
            _DicFormsPaths.Add("FreeModeUI", @"UI\FreeModeUI");
            _DicFormsPaths.Add("TestLevelSelectUI", @"UI\TestLevelSelectUI");
            _DicFormsPaths.Add("TestStartUI", @"UI\TestStartUI");
            _DicFormsPaths.Add("PauseUI", @"UI\PauseUI");
            _DicFormsPaths.Add("EndStageUI", @"UI\EndStageUI");
            _DicFormsPaths.Add("SkillSelectUI", @"UI\SkillSelectUI");
        }
    }


    /// <summary>
    /// 显示（打开）UI窗体
    /// 功能：
    /// 1: 根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
    /// 2: 根据不同的UI窗体的“显示模式”，分别作不同的加载处理
    /// </summary>
    /// <param name="strUIFormName">UI窗体预制体的名称</param>
    public void ShowUIForms(string strUIFormName)
    {
        BaseUIForm baseUIForms = null;                    //UI窗体基类

        //参数的检查
        if (string.IsNullOrEmpty(strUIFormName)) return;

        //根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
        baseUIForms = LoadFormsToAllUIFormsCache(strUIFormName);
        if (baseUIForms == null) return;

        //判断是否清空“栈”结构体集合
        if (baseUIForms.CurrentUIType.IsClearReverseChange)
        {
            if (!ClearStackArray())
            {
                Debug.LogError("栈中数据没有成功清空!!"); 
            }
            
        }

        //判断不同的窗体显示模式，分别进行处理
        switch (baseUIForms.CurrentUIType.UIForm_ShowMode)
        {
            case UIFormShowMode.Normal:
                EnterUIFormsCache(strUIFormName);
                break;
            case UIFormShowMode.ReverseChange:
                PushUIForms(strUIFormName);
                break;
            case UIFormShowMode.HideOther:
                EnterUIFormstToCacheHideOther(strUIFormName);
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// 关闭或返回上一个UI窗体(关闭当前UI窗体)
    /// </summary>
    public void CloseOrReturnUIForms(string strUIFormName)
    {
        BaseUIForm baseUIForms = null;                   //UI窗体基类

        // 参数检查 
        if (string.IsNullOrEmpty(strUIFormName)) return;

        //“所有UI窗体缓存”如果没有记录，则直接返回。
        DicALLUIForms.TryGetValue(strUIFormName, out baseUIForms);
        if (baseUIForms == null) return;

        //判断不同的窗体显示模式，分别进行处理 
        switch (baseUIForms.CurrentUIType.UIForm_ShowMode)
        {
            case UIFormShowMode.Normal:
                ExitUIFormsCache(strUIFormName);
                break;
            case UIFormShowMode.ReverseChange:
                PopUIForms();
                break;
            case UIFormShowMode.HideOther:
                ExitUIFormsFromCacheAndShowOther(strUIFormName);
                break;
            default:
                break;
        }

    }


    public void CloseAll()
    {
        //“正在显示UI窗体缓存”与“栈缓存”集合里所有窗体进行隐藏处理。
        foreach (BaseUIForm baseUIFormsItem in _DicCurrentShowUIForms.Values)
        {
            baseUIFormsItem.Hiding();   
        }
        _DicCurrentShowUIForms.Clear();

        for (int i = 0; i < _StaCurrentUIForms.Count; i++)
        {
            _StaCurrentUIForms.Peek().Hiding();
            _StaCurrentUIForms.Pop();
        }

    }


    /// <summary>
    /// 发动一个弹窗UI
    /// </summary>
    /// <param name="Content"></param>
    public void PopUp(string Content)
    {
        BaseUIForm UIForm = null;

        DicALLUIForms.TryGetValue("PopUpUI", out UIForm);

        if (UIForm == null)
        {
            UIForm = LoadFormsToAllUIFormsCache("PopUpUI");
            if (UIForm == null)
            {
                Debug.LogError("PopUpUI加载失败");
            }
        }

        if (UIForm != null)
        {
            PopUpUI _PopUpUI = UIForm as PopUpUI;
            _PopUpUI.popUpText.text = Content;
            PushUIForms("PopUpUI");
        }
    }

    #region 显示"UI管理器"内部核心数据，测试使用

    /// <summary>
    /// 显示所有UI窗体的数量
    /// </summary>
    /// <returns></returns>
    public int ShowCacheUIsCount()
    {
        return DicALLUIForms?.Count ?? 0;
    }
    /// <summary>
    /// 显示当前窗体的数量
    /// </summary>
    /// <returns></returns>
    public int ShowShowUIsCount()
    {
        return _DicCurrentShowUIForms?.Count ?? 0;
    }
    /// <summary>
    /// 显示栈窗体的数量
    /// </summary>
    /// <returns></returns>
    public int ShowStaUIsCount()
    {
        return _StaCurrentUIForms?.Count ?? 0;
    }
    #endregion


    #region 私有方法
    //初始化加载（根UI窗体）Canvas预设
    private void InitRootCanvasLoading()
    {
        GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("UI/UIRoot", Vector3.zero);
    }


    /// <summary>
    /// 根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
    /// 功能： 检查“所有UI窗体”集合中，是否已经加载过，若没有加载过则新加载窗体
    /// </summary>
    /// <param name="uiFormsName">UI窗体（预设）的名称</param>
    /// <returns></returns>
    private BaseUIForm LoadFormsToAllUIFormsCache(string uiFormsName)
    {
        BaseUIForm baseUIResult = null;                 //加载的返回UI窗体基类

        DicALLUIForms.TryGetValue(uiFormsName, out baseUIResult);
        if (baseUIResult == null)
        {
            //加载指定名称的“UI窗体”
            baseUIResult = LoadUIForm(uiFormsName);
        }

        return baseUIResult;
    }


    /// <summary>
    /// 加载指定名称的“UI窗体”
    /// 功能：
    ///    1：根据“UI窗体名称”，加载预制体。
    ///    2：根据不同预制体中带的脚本中不同的“位置信息”，加载到“根窗体”下不同的节点。
    ///    3：隐藏刚创建的UI克隆体。
    ///    4：把克隆体，加入到“所有UI窗体”（缓存）集合中。
    /// 
    /// </summary>
    /// <param name="uiFormName">UI窗体名称</param>
    private BaseUIForm LoadUIForm(string uiFormName)
    {
        string strUIFormPaths = null;                   //UI窗体路径
        GameObject goCloneUIPrefabs = null;             //创建的UI克隆体预设
        BaseUIForm baseUiForm = null;                     //窗体基类


        //根据UI窗体名称，得到对应的加载路径
        _DicFormsPaths.TryGetValue(uiFormName, out strUIFormPaths);

        //根据“UI窗体名称”，加载“预设克隆体”
        if (!string.IsNullOrEmpty(strUIFormPaths))
        {
            goCloneUIPrefabs = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>(strUIFormPaths, Vector3.zero);
        }

        //设置“UI克隆体”的父节点（根据克隆体中带的脚本中不同的“位置信息”）
        if (_TraCanvasTransfrom != null && goCloneUIPrefabs != null)
        {
            baseUiForm = goCloneUIPrefabs.GetComponent<BaseUIForm>();
            if (baseUiForm == null)
            {
                Debug.Log("baseUiForm==null! ,请先确认窗体预设对象上是否加载了baseUIForm的子类脚本！ 参数 uiFormName=" + uiFormName);
                return null;
            }

            switch (baseUiForm.CurrentUIType.UIForm_Type)
            {
                case UIFormType.Normal: //普通窗体节点
                    goCloneUIPrefabs.transform.SetParent(_TraNormal, false);
                    break;
                case UIFormType.Fixed: //固定窗体节点
                    goCloneUIPrefabs.transform.SetParent(_TraFixed, false);
                    break;
                case UIFormType.PopUp: //弹出窗体节点
                    goCloneUIPrefabs.transform.SetParent(_TraPopUp, false);
                    break;
                default:
                    break;
            }

            //设置隐藏
            //goCloneUIPrefabs.SetActive(false);
            //把克隆体，加入到“所有UI窗体”（缓存）集合中。
            DicALLUIForms.Add(uiFormName, baseUiForm);
            return baseUiForm;
        }
        else
        {
            Debug.Log("_TraCanvasTransfrom==null Or goCloneUIPrefabs==null!! ,Plese Check!, 参数uiFormName=" + uiFormName);
        }

        Debug.Log("出现不可以预估的错误，请检查，参数 uiFormName=" + uiFormName);
        return null;
    }//Mehtod_end


    /// <summary>
    /// 加载UI窗体到“当前显示窗体集合”缓存中。
    /// </summary>
    /// <param name="strUIFormsName"></param>
    private void EnterUIFormsCache(string strUIFormsName)
    {
        BaseUIForm baseUIForms;                        //UI窗体基类
        BaseUIForm baseUIFormsFromAllCache;            //"所有窗体集合"中的窗体基类

        //“正在显示UI窗体缓存”集合里有记录，则直接返回。
        _DicCurrentShowUIForms.TryGetValue(strUIFormsName, out baseUIForms);
        if (baseUIForms != null) return;

        //把当前窗体，加载到“正在显示UI窗体缓存”集合里
        DicALLUIForms.TryGetValue(strUIFormsName, out baseUIFormsFromAllCache);
        if (baseUIFormsFromAllCache != null)
        {
            _DicCurrentShowUIForms.Add(strUIFormsName, baseUIFormsFromAllCache);
            baseUIFormsFromAllCache.Display();
        }
    }


    /// <summary>
    /// 卸载UI窗体从“当前显示窗体集合”缓存中。
    /// </summary>
    /// <param name="strUIFormsName"></param>
    private void ExitUIFormsCache(string strUIFormsName)
    {
        BaseUIForm baseUIForms;                        //UI窗体基类

        //“正在显示UI窗体缓存”集合没有记录，则直接返回。
        _DicCurrentShowUIForms.TryGetValue(strUIFormsName, out baseUIForms);
        if (baseUIForms == null) return;

        //指定UI窗体，运行隐藏状态，且从“正在显示UI窗体缓存”集合中移除。
        baseUIForms.Hiding();
        _DicCurrentShowUIForms.Remove(strUIFormsName);
    }


    /// <summary>
    /// 加载UI窗体到“当前显示窗体集合”缓存中,且隐藏其他正在显示的页面
    /// </summary>
    /// <param name="strUIFormsName"></param>
    private void EnterUIFormstToCacheHideOther(string strUIFormsName)
    {
        BaseUIForm baseUIForms;                        //UI窗体基类
        BaseUIForm baseUIFormsFromAllCache;            //"所有窗体集合"中的窗体基类

        //“正在显示UI窗体缓存”集合里有记录，则直接返回。
        _DicCurrentShowUIForms.TryGetValue(strUIFormsName, out baseUIForms);
        if (baseUIForms != null) return;

        //“正在显示UI窗体缓存”与“栈缓存”集合里所有窗体进行隐藏处理。
        foreach (BaseUIForm baseUIFormsItem in _DicCurrentShowUIForms.Values)
        {
            baseUIFormsItem.Hiding();
        }
        foreach (BaseUIForm basUIFormsItem in _StaCurrentUIForms)
        {
            basUIFormsItem.Hiding();
        }

        //把当前窗体，加载到“正在显示UI窗体缓存”集合里
        DicALLUIForms.TryGetValue(strUIFormsName, out baseUIFormsFromAllCache);
        if (baseUIFormsFromAllCache != null)
        {
            _DicCurrentShowUIForms.Add(strUIFormsName, baseUIFormsFromAllCache);
            baseUIFormsFromAllCache.Display();
        }
    }


    /// <summary>
    /// 卸载UI窗体从“当前显示窗体集合”缓存中,且显示其他原本需要显示的页面
    /// </summary>
    /// <param name="strUIFormsName"></param>
    private void ExitUIFormsFromCacheAndShowOther(string strUIFormsName)
    {
        BaseUIForm baseUIForms;                        //UI窗体基类

        //“正在显示UI窗体缓存”集合没有记录，则直接返回。
        _DicCurrentShowUIForms.TryGetValue(strUIFormsName, out baseUIForms);
        if (baseUIForms == null) return;

        //指定UI窗体，运行隐藏状态，且从“正在显示UI窗体缓存”集合中移除。
        baseUIForms.Hiding();
        _DicCurrentShowUIForms.Remove(strUIFormsName);

        //“正在显示UI窗体缓存”与“栈缓存”集合里所有窗体进行再次显示处理。
        foreach (BaseUIForm baseUIFormsItem in _DicCurrentShowUIForms.Values)
        {
            baseUIFormsItem.Redisplay();
        }
        foreach (BaseUIForm basUIFormsItem in _StaCurrentUIForms)
        {
            basUIFormsItem.Redisplay();
        }
    }


    /// <summary>
    /// UI窗体入栈逻辑
    /// 功能1： 判断栈里是否已经有窗体，有则“冻结”
    ///     2： 先判断“UI预设缓存集合”是否有指定的UI窗体,有则处理。
    ///     3： 指定UI窗体入"栈"
    /// </summary>
    /// <param name="strUIFormsName"></param>
    private void PushUIForms(string strUIFormsName)
    {
        BaseUIForm baseUI;                             //UI预设窗体


        //判断栈里是否已经有窗体，有则“冻结”
        if (_StaCurrentUIForms.Count > 0)
        {
            BaseUIForm topUIForms = _StaCurrentUIForms.Peek();
            topUIForms.Freeze();
        }

        //先判断“UI预设缓存集合”是否有指定的UI窗体,有则处理。
        DicALLUIForms.TryGetValue(strUIFormsName, out baseUI);
        if (baseUI != null)
        {
            baseUI.Display();
        }
        else
        {
            Debug.Log(GetType() + string.Format("/PushUIForms()/ baseUI==null! 核心错误，请检查 strUIFormsName={0} ", strUIFormsName));
        }

        //指定UI窗体入"栈"
        _StaCurrentUIForms.Push(baseUI);
    }


    /// <summary>
    /// UI窗体出栈逻辑
    /// </summary>
    private void PopUIForms()
    {
        if (_StaCurrentUIForms.Count >= 2)
        {
            /* 出栈逻辑 */
            BaseUIForm topUIForms = _StaCurrentUIForms.Pop();
            //出栈的窗体，进行隐藏处理
            topUIForms.Hiding();
            //出栈窗体的下一个窗体逻辑
            BaseUIForm nextUIForms = _StaCurrentUIForms.Peek();
            //下一个窗体"重新显示"处理
            nextUIForms.Redisplay();
        }
        else if (_StaCurrentUIForms.Count == 1)
        {
            /* 出栈逻辑 */
            BaseUIForm topUIForms = _StaCurrentUIForms.Pop();
            //出栈的窗体，进行"隐藏"处理
            topUIForms.Hiding();
        }

    }


    /// <summary>
    /// 清空栈
    /// </summary>
    /// <returns></returns>
    private bool ClearStackArray()
    {
        if (_StaCurrentUIForms != null && _StaCurrentUIForms.Count >= 1)
        {
            _StaCurrentUIForms.Clear();
            return true;
        }

        return false;
    }




    #endregion

}//class_end