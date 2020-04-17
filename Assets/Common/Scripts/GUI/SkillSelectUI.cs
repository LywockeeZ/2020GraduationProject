using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;
using DG.Tweening;

public class SkillSelectUI : BaseUIForm
{
    public List<MMTouchButton> skillBtns;
    public List<MMTouchButton> itemBtns;
    public MMTouchButton mainSkillSlot;
    public List<MMTouchButton> itemSlots;

    private Dictionary<string, MMTouchButton> btns;

    private void Start()
    {
        btns.Add("whirlwind", skillBtns[0]);
        LoadUnLockSkill();        
    }

    /// <summary>
    /// 加载已经解锁的技能
    /// </summary>
    private void LoadUnLockSkill()
    {

    }

    /// <summary>
    /// 检查待解锁的技能并解锁
    /// </summary>
    public void CheckSkill()
    {

    }

    /// <summary>
    /// 由锁定状态变为未选择状态
    /// </summary>
    public void LockToUnSelect(MMTouchButton targetBtn)
    {
        UITool.GetUIComponent<Image>(targetBtn.gameObject, "Background").color = Color.clear;
        UnityTool.FindChildGameObject(targetBtn.gameObject, "Btn_unlockSkill").SetActive(false);
        targetBtn.EnableButton();
    }

    /// <summary>
    /// 由未选择状态变为选择状态
    /// </summary>
    public void UnSelectToSelect()
    {

    }

    /// <summary>
    /// 由选择状态变为未选择状态
    /// </summary>
    public void SelectToUnSelect()
    {

    }
}

