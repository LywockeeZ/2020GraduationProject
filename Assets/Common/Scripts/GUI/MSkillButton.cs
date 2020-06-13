using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MoreMountains.Tools;

public enum SkillButtonStates { Locked, Normal, Selected, Unlocking, Disabled };
public class MSkillButton : MMTouchButton
{

    [Header("Skill Setting")]
    public SkillButtonStates SkBtnState;//{ get; protected set; }
    public SkillButtonStates defaultState;
    public string itemName;
    public ISelectItem seletMenu;
    public MMTouchButton LockBtn;
    public Image preview;
    public Sprite previewSprite;

    private bool isUnlocking = false;
    private SkillButtonStates lastState;
    private bool isFirstOpen = true;

    protected override void OnEnable()
    {
        if (!isFirstOpen)
        {
            ResetButton();
        }
    }


    private void OnDisable()
    {
        lastState = SkBtnState;
        if (SkBtnState == SkillButtonStates.Selected)
        {
            seletMenu.SelectCancel();
        }
    }


    protected override void Initialization()
    {
        //在按钮状态初始化前，设置为默认状态，因为base的初始化方法中有重置按钮方法，必须在重置之前设置为defaultstate
        lastState = defaultState;
        if (preview != null)
        {
            preview.color = Color.clear;
        }
        
        base.Initialization();
    }

    private void Start()
    {
        isFirstOpen = false;
        seletMenu.SelectedButton = null;
    }

    protected override void Update()
    {
        switch (SkBtnState)
        {
            case SkillButtonStates.Locked:
                LockBtn.EnableButton();
                DisableButton();
                break;
            case SkillButtonStates.Unlocking:
                //执行解锁动画部分
                if (isUnlocking)
                {
                    //void action() { SkBtnState = SkillButtonStates.Locked; }
                    //LockBtn.Animator.SetBool("Unlocking", true, action);
                    SkBtnState = SkillButtonStates.Locked;
                    isUnlocking = false;
                }
                break;
            case SkillButtonStates.Normal:
                seletMenu.SelectedButton = null;
                if (preview != null)
                {
                    preview.color = Color.clear;
                }
                LockBtn.DisableButton();
                EnableButton();
                break;
            case SkillButtonStates.Selected:
                CurrentState = ButtonStates.ButtonPressed;
                if (preview!= null)
                {
                    preview.sprite = previewSprite;
                    preview.color = Color.white;
                }
                break;
            case SkillButtonStates.Disabled:
                LockBtn.DisableButton();
                DisableButton();
                break;
            default:
                break;
        }

        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        //进入选中状态,进入瞬间调用
        if (Input.GetMouseButtonDown(0) && SkBtnState == SkillButtonStates.Normal && isInArea)
        {
            SkBtnState = SkillButtonStates.Selected;
            seletMenu.Select(this);
            ButtonSelectBegain?.Invoke();
            return;
        }
        //进入取消状态，进入瞬间调用
        if ((Input.GetMouseButtonDown(1) && SkBtnState == SkillButtonStates.Selected) ||
            (Input.GetMouseButtonDown(0) && SkBtnState == SkillButtonStates.Selected && isInArea))
        {
            SkBtnState = SkillButtonStates.Normal;
            if (isInArea)
            {
                CurrentState = ButtonStates.ButtonEnter;
            }
            else
                CurrentState = ButtonStates.Off;
            seletMenu.SelectCancel();
            ButtonSelectCancel?.Invoke();
            return;
        }


        //选中状态关闭界面再打开时，使按键恢复正常
        if (CurrentState == ButtonStates.ButtonPressed)
        {
            CurrentState = ButtonStates.Off;
        }

    }

    /// <summary>
    /// 初始化按钮状态
    /// </summary>
    protected override void ResetButton()
    {
        base.ResetButton();
        if (lastState != SkillButtonStates.Selected && lastState != SkillButtonStates.Disabled)
        {
            SkBtnState = lastState;
        }
        else SkBtnState = SkillButtonStates.Normal;
    }

    /// <summary>
    /// 当触摸进入区域时触发绑定指针进入操作
    /// </summary>
    public override void OnPointerEnter(PointerEventData data)
    {
        if (!MouseMode)
        {
            OnPointerDown(data);
        }
        else
        {
            if (CurrentState == ButtonStates.Off)
            {
                CurrentState = ButtonStates.ButtonEnter;
                if (SkBtnState == SkillButtonStates.Normal)
                {
                    ButtonEnteredFirstTime?.Invoke();
                }
            }
        }
        isInArea = true;
    }



    public virtual void Normal()
    {
        SkBtnState = SkillButtonStates.Normal;
    }

    public virtual void Disabled()
    {
        SkBtnState = SkillButtonStates.Disabled;
    }

    public virtual void Locked()
    {
        //CancelSelect();
        SkBtnState = SkillButtonStates.Locked;
        lastState = SkBtnState;
    }

    public virtual void Unlocked()
    {
        SkBtnState = SkillButtonStates.Normal;
        lastState = SkBtnState;
    }

    public virtual void Unlocking()
    {
        isUnlocking = true;
        SkBtnState = SkillButtonStates.Unlocking;
    }

    public virtual void SetSelectMeun(ISelectItem selectUI)
    {
        seletMenu = selectUI;
    }

    public virtual void Unselectable()
    {
        _selectable.interactable = false;
    }

    public virtual void Selectable()
    {
        _selectable.interactable = true;
    }

    /// <summary>
    /// 直接强制转换状态时，先检查是不是在选择状态，然后取消选择
    /// </summary>
    private void CancelSelect()
    {
        if (SkBtnState == SkillButtonStates.Selected)
        {
            seletMenu.SelectCancel();
        }
    }
}
