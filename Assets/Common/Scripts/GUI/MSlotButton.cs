using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MoreMountains.Tools;

public class MSlotButton : MMTouchButton
{
    public enum SlotButtonStates { Unequiped, Equiped, Disabled, Locked}

    [Header("Slot Setting")]
    public MMTouchButton LockBtn;
    public SkillSelectUI selectMenu;
    public Image skillIcon;
    public Sprite cancelSprite;
    public Color cancelSpriteColor = Color.white;

    [HideInInspector]
    public MSkillButton equipedItem;

    public SlotButtonStates slotState;
    private SlotButtonStates lastState;

    protected Sprite _initialEnterSprite;
    protected Sprite _initialSlotSprite;
    protected Color _initialEnterColor;
    protected Color _initialSlotColor;

    protected override void Initialization()
    {
        base.Initialization();
        _initialEnterSprite = EnteredSprite;
        _initialSlotSprite = _initialSprite;
        _initialSlotColor = _initialColor;
        _initialEnterColor = EnteredColor;
    }

    protected override void OnEnable()
    {
        ResetButton();
    }

    protected override void Update()
    {
        switch (slotState)
        {
            case SlotButtonStates.Unequiped:
                LockBtn.DisableButton();
                EnableButton();
                skillIcon.color = Color.clear;
                _initialSprite = _initialSlotSprite;
                _initialColor = _initialSlotColor;
                EnteredSprite = _initialEnterSprite;
                EnteredColor = _initialEnterColor;
                break;
            case SlotButtonStates.Equiped:
                EnableButton();
                skillIcon.color = Color.white;
                _initialColor = Color.clear;
                EnteredSprite = cancelSprite;
                EnteredColor = cancelSpriteColor;
                break;
            case SlotButtonStates.Disabled:
                DisableButton();
                break;
            case SlotButtonStates.Locked:
                skillIcon.color = Color.clear;
                _initialSprite = _initialSlotSprite;
                _initialColor = _initialSlotColor;
                EnteredSprite = _initialEnterSprite;
                EnteredColor = _initialEnterColor;
                DisableButton();
                LockBtn.EnableButton();
                break;
            default:
                break;
        }

        base.Update();

    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (Input.GetMouseButtonDown(0) && isInArea && slotState == SlotButtonStates.Unequiped)
        {
            EquipCheck();
            return;
        }

        if (Input.GetMouseButtonDown(0) && isInArea && slotState == SlotButtonStates.Equiped)
        {
            EquipCheck();
            return;
        }

    }


    /// <summary>
    /// 初始化按钮状态
    /// </summary>
    protected override void ResetButton()
    {
        base.ResetButton();
        SetState(SlotButtonStates.Unequiped);
        equipedItem = null;
    }

    public void Disabled()
    {
        lastState = slotState;
        slotState = SlotButtonStates.Disabled;
    }

    public void Enabled()
    {
        slotState = lastState;
    }

    public void EquipCheck()
    {
        //未选中时也可以删除
        if (selectMenu.selectedButton == null)
        {
            if (slotState == SlotButtonStates.Equiped)
            {
                Unequip();
                return;
            }
        }
        else
        {
            if (slotState == SlotButtonStates.Unequiped)
            {
                Equip();
                return;
            }

            if (slotState == SlotButtonStates.Equiped)
            {
                Unequip();
                return;
            }
        }

    }

    public void Equip()
    {
        SetState(SlotButtonStates.Equiped);
        skillIcon.sprite = selectMenu.selectedButton.DisabledSprite;
        equipedItem = selectMenu.selectedButton;
        OnEquipComplete();
        Debug.Log("Equiped:" + equipedItem.itemName);
    }

    public void Unequip()
    {
        Debug.Log("Unequip:" + equipedItem.itemName);
        SetState(SlotButtonStates.Unequiped);
        equipedItem = null;
    }

    public void OnEquipComplete()
    {
        selectMenu.selectedButton = null;
        selectMenu.SelectComplete();
    }

    public void SetState(SlotButtonStates states)
    {
        slotState = states;
        lastState = states;
    }


}
