using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MoreMountains.Tools;

public class MSkillButton : MMTouchButton
{
    public enum SkillButtonStates { Locked, Normal, Selected, Unlocking, Disabled};

    [Header("Skill Setting")]
    public SkillButtonStates SkBtnState;//{ get; protected set; }
    [HideInInspector]
    public string itemName;

    public SkillSelectUI seletMenu;
    public MMTouchButton LockBtn;
    public Image preview;
    public Sprite previewSprite;

    private bool isUnlocking = false;

    protected override void OnEnable()
    {
        ResetButton();
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
                    void action() { SkBtnState = SkillButtonStates.Locked; }
                    LockBtn.Animator.SetBool("Unlocking", true, action);
                    isUnlocking = false;
                }
                break;
            case SkillButtonStates.Normal:
                LockBtn.DisableButton();
                EnableButton();
                break;
            case SkillButtonStates.Selected:
                CurrentState = ButtonStates.ButtonPressed;
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
        //进入选中状态
        if (Input.GetMouseButtonDown(0) && SkBtnState == SkillButtonStates.Normal && isInArea)
        {
            seletMenu.Select(this);
            SkBtnState = SkillButtonStates.Selected;
            preview.sprite = previewSprite;
            preview.color = Color.white;
            return;
        }
        //进入取消状态
        if (Input.GetMouseButtonDown(1) && SkBtnState == SkillButtonStates.Selected ||
            Input.GetMouseButtonDown(0) && SkBtnState == SkillButtonStates.Selected && isInArea)
        {
            SkBtnState = SkillButtonStates.Normal;
            if (isInArea)
            {
                CurrentState = ButtonStates.ButtonEnter;
            }
            else
                CurrentState = ButtonStates.Off;
            preview.color = Color.clear;
            seletMenu.SelectCancel();
        }

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
        SkBtnState = SkillButtonStates.Normal;
        preview.color = Color.clear;
        seletMenu.selectedButton = null;
    }


    public virtual void Normal()
    {
        SkBtnState = SkillButtonStates.Normal;
    }

    public virtual void Disabled()
    {
        SkBtnState = SkillButtonStates.Disabled;
    }

    public virtual void Unlocked()
    {
        SkBtnState = SkillButtonStates.Normal;
    }

    public virtual void Unlocking()
    {
        isUnlocking = true;
        SkBtnState = SkillButtonStates.Unlocking;
    }
}
