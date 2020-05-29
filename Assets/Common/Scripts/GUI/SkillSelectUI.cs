using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;
using DG.Tweening;

public class SkillSelectUI : BaseUIForm, ISelectItem
{
    public List<MSkillButton> skillBtns;
    public List<MSkillButton> itemBtns;
    public MSlotButton mainSkillSlot;
    public List<MSlotButton> itemSlots;
    public MSkillButton SelectedButton { get; set; }
    public Image preview_Level;
    public Image title;

    private Dictionary<string, MSkillButton> btns = new Dictionary<string, MSkillButton>();
    private bool isFirstOpen = true;

    public void OnEnable()
    {
        if (!isFirstOpen)
        {
            CheckSkill();
        }

        SetLevelPreviewAndTitle();
        Game.Instance.SetCanInput(false);
    }

    public void OnDisable()
    {
        void action()
        {
            Game.Instance.SetCanInput(true);
        }
        CoroutineManager.StartCoroutineTask(action, 0.5f);
    }

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.Normal;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Pentrate;
        SetSelectMenuToAll();
    }

    private void Start()
    {
        AddButton("skill_Whirlwind", skillBtns[0]);
        AddButton("skill_Slash", skillBtns[1]);
        AddButton("item_WaterSac", itemBtns[0]);
        LoadUnLockSkill();
        CheckSkill();
        //TestAddAll();
        //TestUnlockAll();
        isFirstOpen = false;
    }


    /// <summary>
    /// 加载已经解锁的技能
    /// </summary>
    private void LoadUnLockSkill()
    {
        Dictionary<string, SkillInstanceBase> unlockedSkills = Game.Instance.GetUnlockSkills();
        foreach (var skill in unlockedSkills)
        {
            btns.TryGetValue(skill.Key, out MSkillButton button);
            if (button != null)
            {
                button.Unlocked();
            }
        }
    }

    /// <summary>
    /// 检查待解锁的技能并解锁
    /// </summary>
    public void CheckSkill()
    {
        Queue<SkillInstanceBase> skillsToUnlock = Game.Instance.GetSkillsToUnlock();
        while (skillsToUnlock.Count != 0)
        {
            SkillInstanceBase skill = skillsToUnlock.Dequeue();
            btns.TryGetValue(skill.SkillName, out MSkillButton button);
            //button?.Unlocking();
            button.Unlocked();
            Debug.Log(button.SkBtnState);

        }
    }

    /// <summary>
    /// 设置关卡预览界面和关卡标题
    /// </summary>
    public void SetLevelPreviewAndTitle()
    {
        if (Game.Instance.GetCurrentStage()!= null && !Game.Instance.isTest)
        {
            preview_Level.sprite = GameFactory.GetAssetFactory().LoadAsset<Sprite>(Game.Instance.GetCurrentStage().levelPreviewUIPath + Game.Instance.GetLevelWillToOnMain());

            title.sprite = GameFactory.GetAssetFactory().LoadAsset<Sprite>(Game.Instance.GetCurrentStage().titleUIPath + Game.Instance.GetLevelWillToOnMain());

        }
    }

    /// <summary>
    /// 选中后，其他技能按钮都设置为不可用
    /// </summary>
    /// <param name="mSkillButton"></param>
    public void Select(MSkillButton mSkillButton)
    {
        Debug.Log("Select:" + mSkillButton.itemName);
        SelectedButton = mSkillButton;
        for (int i = 0; i < skillBtns.Count; i++)
        {
            if (skillBtns[i] != mSkillButton)
            {
                if (skillBtns[i].SkBtnState != MSkillButton.SkillButtonStates.Locked)
                {
                    skillBtns[i].Disabled();
                }
            }
            else
                DisableItemSlot();

        }

        for (int i = 0; i < itemBtns.Count; i++)
        {
            if (itemBtns[i] != mSkillButton)
            {
                if (itemBtns[i].SkBtnState != MSkillButton.SkillButtonStates.Locked)
                {
                    itemBtns[i].Disabled();
                }
            }
            else
                DisableSkillSlot();
        }

    }

    /// <summary>
    /// 全部恢复为可选状态
    /// </summary>
    public void SelectComplete()
    {
        for (int i = 0; i < skillBtns.Count; i++)
        {
            if (skillBtns[i].SkBtnState != MSkillButton.SkillButtonStates.Locked)
            {
                skillBtns[i].Normal();
            }
        }

        for (int i = 0; i < itemBtns.Count; i++)
        {
            if (itemBtns[i].SkBtnState != MSkillButton.SkillButtonStates.Locked)
            {
                itemBtns[i].Normal();
            }
        }

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].slotState != MSlotButton.SlotButtonStates.Locked &&
                itemSlots[i].slotState != MSlotButton.SlotButtonStates.Equiped)
            {
                itemSlots[i].Enabled();
            }
        }

        if (mainSkillSlot.slotState != MSlotButton.SlotButtonStates.Equiped &&
            mainSkillSlot.slotState != MSlotButton.SlotButtonStates.Locked)
        {
            mainSkillSlot.Enabled();
        }
    }

    /// <summary>
    /// 取消选中时恢复为可用状态,并清空已选中按钮
    /// </summary>
    public void SelectCancel()
    {
        Debug.Log("SelectCancel:"+SelectedButton?.itemName);
        SelectedButton = null;
        SelectComplete();
    }

    public void DisableItemSlot()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].Disabled();
        }
    }

    public void DisableSkillSlot()
    {
        mainSkillSlot.Disabled();
    }

    public void SetSelectMenuToAll()
    {
        for (int i = 0; i < skillBtns.Count; i++)
        {
            skillBtns[i].SetSelectMeun(this);
        }

        for (int i = 0; i < itemBtns.Count; i++)
        {
            itemBtns[i].SetSelectMeun(this);
        }

    }

    public void EnterButton()
    {
        List<string> itemNames = new List<string>();
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].equipedItem != null)
                itemNames.Add(itemSlots[i].equipedItem.itemName);
            else itemNames.Add(null);
        }
        Game.Instance.GetMainItems()?.Clear();
        Game.Instance.SetMainItems(itemNames);
        Game.Instance.SetMainSkill(mainSkillSlot.equipedItem?.itemName);
        Game.Instance.LoadLevelOnMain(Game.Instance.GetSceneWillToOnMain(), Game.Instance.GetLevelWillToOnMain());
        Game.Instance.CloseUI("SkillSelectUI");
    }

    public void BackButton()
    {
        Game.Instance.CloseUI("SkillSelectUI");
    }


    public void TestUnlockAll()
    {
        for (int i = 0; i < skillBtns.Count; i++)
        {
            skillBtns[i].Unlocked();
        }

        for (int i = 0; i < itemBtns.Count; i++)
        {
            itemBtns[i].Unlocked();
        }

    }

    public void TestAddAll()
    {
        AddButton("A", skillBtns[0]);
        AddButton("B", skillBtns[1]);
        AddButton("1", itemBtns[0]);
        AddButton("2", itemBtns[1]);
        AddButton("3", itemBtns[2]);
    }

    /// <summary>
    /// 在此界面控制器上注册按钮
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="button"></param>
    public void AddButton(string itemName, MSkillButton button)
    {
        btns.Add(itemName, button);
        button.itemName = itemName;
    }

}

