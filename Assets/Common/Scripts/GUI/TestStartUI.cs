using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

public class TestStartUI : BaseUIForm
{
    public Text messageText;
    public GameObject skillItem;
    public GameObject skillSelectBar;

    private Dictionary<string, SkillInstanceBase> m_UnlockedSkills;
    private Dictionary<string, MMTouchButton> m_SkillSelectBtns = new Dictionary<string, MMTouchButton>();
    private List<GameObject> m_SkillIcon = new List<GameObject>();

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.HideOther;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Lucency;

        m_UnlockedSkills = Game.Instance.GetUnlockSkills();
        messageText = UITool.GetUIComponent<Text>(this.gameObject, "Txt_panel");
        skillItem = UnityTool.FindChildGameObject(this.gameObject, "SkillItem");
        skillSelectBar = UnityTool.FindChildGameObject(this.gameObject, "Bar_skillSelect");
    }

    private void OnEnable()
    {
        if (m_UnlockedSkills.Count != 0)
        {
            skillSelectBar.SetActive(true);
            foreach (var item in m_UnlockedSkills)
            {
                var skillIconObj = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>(
                    "UI/UIComponent/SkillSelectIcon/" + item.Key, Vector3.zero);
                var skillBtn = UITool.GetUIComponent<MMTouchButton>(skillIconObj, "Background");
                skillIconObj.transform.SetParent(skillItem.transform);
                skillIconObj.transform.localScale = new Vector3(1, 1, 1);
                void action() { OnSkillSelect(skillBtn, item.Key); }
                skillBtn.ButtonPressedFirstTime.AddListener(action);
                m_SkillIcon.Add(skillIconObj);
                m_SkillSelectBtns.Add(item.Key, skillBtn);
            }
        }
        else skillSelectBar.SetActive(false);
    }

    private void OnDisable()
    {
        for (int i = 0 ; i < m_SkillIcon.Count; i++)
        {
            Destroy(m_SkillIcon[i]);
        }
        m_SkillIcon.Clear();
        m_SkillSelectBtns.Clear();
    }

    /// <summary>
    /// 开始按钮调用
    /// </summary>
    public void BtnFunc()
    {
        Game.Instance.CloseUI("TestStartUI");
        Game.Instance.NotifyEvent(ENUM_GameEvent.RoundBegain);
    }

    /// <summary>
    /// 技能选择界面选中技能时调用
    /// </summary>
    /// <param name="button"></param>
    public void OnSkillSelect(MMTouchButton button, string skillName)
    {
        button.DisableButton();
        Game.Instance.SetMainSkill(skillName);
        foreach (var item in m_SkillSelectBtns)
        {
            if (item.Value != button)
            {
                item.Value.gameObject.GetComponent<CanvasGroup>().interactable = false;
            }
        }
    }

    public override void ShowMessage(string content)
    {
        messageText.text = content;
    }


}
