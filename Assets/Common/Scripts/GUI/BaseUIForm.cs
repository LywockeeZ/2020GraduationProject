using UnityEngine;
using DG.Tweening;

public class BaseUIForm : MonoBehaviour
{
    /*  字段  */
    //当前(基类)窗口的类型
    private UIType _CurrentUIType = new UIType();
    protected CanvasGroup canvasGroup;
    /*  属性  */
    /// <summary>
    /// 属性_当前UI窗体类型
    /// </summary>
    internal UIType CurrentUIType
    {
        set
        {
            _CurrentUIType = value;
        }

        get
        {
            return _CurrentUIType;
        }
    }


    //页面显示
    public virtual void Display()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.DOFade(1, 0.25f).ChangeStartValue(0);
        Open();
    }


    //页面隐藏(不在“栈”集合中)
    public virtual void Hiding()
    {
        canvasGroup.DOFade(0, 0.25f).ChangeStartValue(1).OnComplete(Close);
    }


    //页面重新显示
    public virtual void Redisplay()
    {
        Display();
    }


    //页面冻结(还在“栈”集合中)
    public virtual void Freeze()
    {
        this.gameObject.SetActive(true);
    }


    /// <summary>
    /// 外部调用来改变text文本框信息
    /// </summary>
    /// <param name="content"></param>
    public virtual void ShowMessage(string content)
    {
        Debug.LogWarning("此UI界面未重写ShowMessage方法");
    }

    protected void Open()
    {
        this.gameObject.SetActive(true);

        //设置模态窗体调用(必须是弹出窗体)
        if (_CurrentUIType.UIForm_Type == UIFormType.PopUp)
        {
            UIMaskManager.Instance.SetMaskWindow(this.gameObject, _CurrentUIType.UIForm_LucencyType);
        }
    }

    protected void Close()
    {
        this.gameObject.SetActive(false);

        //取消模态窗体调用
        if (_CurrentUIType.UIForm_Type == UIFormType.PopUp)
        {
            UIMaskManager.Instance.CancelMaskWindow();
        }
    }

}//Class_end