using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBarUI : BaseUIForm, ISelectItem
{
    public readonly string buttonPath = "UI/UIComponent/SkillMainIcon/Btn_";

    public MSkillButton SelectedButton { get; set; }

    public List<Transform> slotsTrans;
    //存放按钮对象
    private List<GameObject> buttonsObj = new List<GameObject>();
    //存放按钮组件
    private List<MSkillButton> buttons = new List<MSkillButton>();
    private bool isFirstOpen = true;

    private void OnEnable()
    {
        if (!isFirstOpen)
        {
            LoadSkill();
            //TestLoadSkill();
        }
        RegisterEvent();
    }

    private void OnDisable()
    {
        ClearSkill();
        DetachEvent();
    }

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.Normal;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Pentrate;

        LoadSkill();
        //TestLoadSkill();
    }

    private void Start()
    {
        isFirstOpen = false;
    }

    private EventListenerDelegate OnRoundBegain;
    private EventListenerDelegate OnSkillEnd;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundBegain,
            OnRoundBegain = (Message evt) =>
            {
                //if (buttons.Count == 4)
                //    buttons[3]?.Normal();
            });

        Game.Instance.RegisterEvent(ENUM_GameEvent.SkillEnd,
            OnSkillEnd = (Message evt) =>
            {
                SelectCancel();
            });
    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundBegain, OnRoundBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.SkillEnd, OnSkillEnd);
    }


    public void LoadSkill()
    {
        SkillInstanceBase mainSkill = Game.Instance.GetMainSkill();
        List<SkillInstanceBase> items = Game.Instance.GetMainItems();

        for (int i = 0; i < items.Count; i++)
        {
            //可以在实例化按钮时，将技能方法注册给技能
            //鼠标进入，离开按钮，分别开启，关闭技能指示器的刷新（增加一个事件OnEnterButton）
            //鼠标按下状态时，持续刷新技能释放器|（OnBunttonPressed）()=>{如果技能状态为未执行，则显示技能释放器，否则关闭按钮}再注册一个事件，当回合结束时重启技能按钮，而不重启物品
            //技能释放后关闭按钮
            //skillIndicator，skillEmitter
            InstantiateButton(items[i], slotsTrans[i]);
        }

        if (mainSkill != null)
            InstantiateButton(mainSkill, slotsTrans[3]);
    }

    public void ClearSkill()
    {
        for (int i = 0; i < buttonsObj.Count; i++)
        {
            if (buttonsObj[i] != null)
            {
                Destroy(buttonsObj[i]);
            }
        }
        buttonsObj.Clear();
        buttons.Clear();
    }

    public void DisableAll()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] != null)
            {
                if (buttons[i].SkBtnState != MSkillButton.SkillButtonStates.Locked)
                {
                    buttons[i].Disabled();
                }
            }

        }
    }


    public void Select(MSkillButton mSkillButton)
    {
        Debug.Log("Select:" + mSkillButton.itemName);
        SelectedButton = mSkillButton;
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] != null && buttons[i] != mSkillButton)
            {
                if (buttons[i].SkBtnState != MSkillButton.SkillButtonStates.Locked)
                {
                    buttons[i].Disabled();
                }
            }

        }
    }

    public void SelectCancel()
    {
        Debug.Log("SelectCancel:" + SelectedButton?.itemName);
        SelectedButton = null;
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] != null && buttons[i].SkBtnState != MSkillButton.SkillButtonStates.Locked)
            {
                buttons[i].Normal();
            }
        }
    }

    public void InstantiateButton(SkillInstanceBase skillInstance, Transform trans)
    {
        if (skillInstance != null)
        {
            GameObject buttonItem = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>(buttonPath + skillInstance.SkillName, trans.position);
            MSkillButton button = buttonItem.transform.GetChild(0).GetChild(0).GetComponent<MSkillButton>();
            buttonItem.transform.SetParent(trans);
            buttonItem.transform.localScale = Vector3.one;
            button.SetSelectMeun(this);
            buttonsObj.Add(buttonItem);
            buttons.Add(button);

            //注册行为
            //进入状态显示技能指示器
            button.ButtonEnteredFirstTime.AddListener(() =>
            {
                skillInstance.ShowIndicator();
            });
            button.ButtonLeaved.AddListener(() =>
            {
                skillInstance.CloseIndicator();
            });

            //选中状态显示技能发射器
            button.ButtonSelectBegain.AddListener(() =>
            {
                //按下的第一帧，显示发射器
                if (skillInstance.m_SkillState == SkillState.Ready)
                {
                    skillInstance.ShowEmitter();
                }
            });
            button.ButtonPressed.AddListener(() =>
            {
                //如果未执行，显示发射器，执行了则将按钮锁定，再根据回合事件开启
                if (skillInstance.m_SkillState != SkillState.Ready)
                {
                    button.Locked();
                }
            });

            button.ButtonSelectCancel.AddListener(() =>
            {
                skillInstance.CloseEmitter();
            });
        }
        else
        {
            buttonsObj.Add(null);
            buttons.Add(null);
        }

    }

    public void TestLoadSkill()
    {
        //InstantiateButton("item_A", slotsTrans[0]);
        //InstantiateButton("item_B", slotsTrans[1]);
        //InstantiateButton("item_C", slotsTrans[2]);
        //InstantiateButton("skill_A", slotsTrans[3]);
    }
}
