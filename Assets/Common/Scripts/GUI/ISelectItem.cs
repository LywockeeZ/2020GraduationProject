using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public interface ISelectItem 
{
    /// <summary>
    /// 控制被选择物品的菜单
    /// </summary>
    MSkillButton SelectedButton { get; set; }

    void Select(MSkillButton mSkillButton);

    void SelectCancel();
}
