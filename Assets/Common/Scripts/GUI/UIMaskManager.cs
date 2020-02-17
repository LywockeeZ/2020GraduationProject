using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIMaskManager : Singleton<UIMaskManager>
{
    /*  字段 */
    //UI根节点对象
    private GameObject _GoCanvasRoot = null;
    //UI脚本节点对象
    private Transform _TraUIScriptsNode = null;
    //顶层面板
    private GameObject _GoTopPanel;
    //遮罩面板
    private GameObject _GoMaskPanel;
    //UI摄像机
    private Camera _UICamera;
    //UI摄像机原始的“层深”
    private float _OriginalUICameralDepth;




    protected override void Awake()
    {
        base.Awake();

        //得到UI根节点对象、脚本节点对象
        _GoCanvasRoot = UnityTool.FindGameObject("UIRoot");
        _TraUIScriptsNode = _GoCanvasRoot.transform.Find("ScriptManager");

        //把本脚本实例，作为“脚本节点对象”的子节点。
        this.gameObject.transform.SetParent(_TraUIScriptsNode, false);
        this.gameObject.name = "UIMaskManager";

        //得到“顶层面板”、“遮罩面板”
        _GoTopPanel = _GoCanvasRoot;
        _GoMaskPanel = UnityTool.FindChildGameObject(_GoCanvasRoot, "UIMaskPanel");

        //得到UI摄像机原始的“层深”
        _UICamera = UnityTool.FindGameObject("UICamera").GetComponent<Camera>();
        if (_UICamera != null)
        {
            //得到UI摄像机原始“层深”
            _OriginalUICameralDepth = _UICamera.depth;
        }
        else
        {
            Debug.Log(GetType() + "/Start()/UI_Camera is Null!,Please Check! ");
        }
    }

    /// <summary>
    /// 设置遮罩状态
    /// </summary>
    /// <param name="goDisplayUIForms">需要显示的UI窗体</param>
    /// <param name="lucenyType">显示透明度属性</param>
    public void SetMaskWindow(GameObject goDisplayUIForms, UIFormLucencyType lucenyType = UIFormLucencyType.Lucency)
    {
        //顶层窗体下移(最后渲染)
        _GoTopPanel.transform.SetAsLastSibling();
        //启用遮罩窗体以及设置透明度
        switch (lucenyType)
        {
            //完全透明，不能穿透
            case UIFormLucencyType.Lucency:
                //print("UI为完全透明");
                _GoMaskPanel.SetActive(true);
                Color newColor1 = new Color(255 / 255F, 255 / 255F, 255 / 255F, 0F / 255F);
                _GoMaskPanel.GetComponent<Image>().color = newColor1;
                break;
            //半透明，不能穿透
            case UIFormLucencyType.Translucence:
                //print("UI为半透明");
                _GoMaskPanel.SetActive(true);
                Color newColor2 = new Color(220 / 255F, 220 / 255F, 220 / 255F, 50 / 255F);
                _GoMaskPanel.GetComponent<Image>().color = newColor2;
                break;
            //低透明，不能穿透
            case UIFormLucencyType.ImPenetrable:
                //print("UI为低透明");
                _GoMaskPanel.SetActive(true);
                Color newColor3 = new Color(50 / 255F, 50 / 255F, 50 / 255F, 200F / 255F);
                _GoMaskPanel.GetComponent<Image>().color = newColor3;
                break;
            //可以穿透
            case UIFormLucencyType.Pentrate:
                //print("UI为允许穿透");
                if (_GoMaskPanel.activeInHierarchy)
                {
                    _GoMaskPanel.SetActive(false);
                }
                break;

            default:
                break;
        }



        //遮罩窗体下移
        _GoMaskPanel.transform.SetAsLastSibling();
        //显示窗体的下移
        goDisplayUIForms.transform.SetAsLastSibling();
        //增加当前UI摄像机的层深（保证当前摄像机为最前显示）
        if (_UICamera != null)
        {
            _UICamera.depth = _UICamera.depth + 100;    //增加层深
        }

    }

    /// <summary>
    /// 取消遮罩状态
    /// </summary>
    public void CancelMaskWindow()
    {
        //顶层窗体上移
        _GoTopPanel.transform.SetAsFirstSibling();
        //禁用遮罩窗体
        if (_GoMaskPanel.activeInHierarchy)
        {
            //隐藏
            _GoMaskPanel.SetActive(false);
        }

        //恢复当前UI摄像机的层深 
        if (_UICamera != null)
        {
            _UICamera.depth = _OriginalUICameralDepth;  //恢复层深
        }
    }
}

