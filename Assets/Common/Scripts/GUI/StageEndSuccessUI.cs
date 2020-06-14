using MoonSharp.Interpreter.Compatibility.Frameworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageEndSuccessUI : BaseUIForm, ISelectItem
{
    public Image img_map;
    public Text txt_ap;
    public List<Image> maps = new List<Image>();
    public List<Transform> slotsTrans;

    private List<SkillButtonStates> buttonStates;
    private bool isClick = false;
    public readonly string buttonPath = "UI/UIComponent/SkillEndIcon/Btn_";

    public MSkillButton SelectedButton { get; set; }

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.Normal;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.HideOther;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Lucency;
    }

    private void OnEnable()
    {
        isClick = false;
        LoadData();
    }

    private void OnDisable()
    {
        for (int i = 0; i < slotsTrans.Count; i++)
        {
            if (slotsTrans[i].childCount != 0)
            {
                Destroy(slotsTrans[i].GetChild(0).gameObject);
            }         
        }
        Game.Instance.SetMainSkill(null);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isClick)
        {
            isClick = true;
            DoNext();
        }
    }

    public virtual void DoNext()
    {
        if (Game.Instance.isTest)
        {
            Game.Instance.CloseUI("StageEndSuccessUI");
            Game.Instance.LoadLevel("LevelSelector");
        }
        else
        {
            Game.Instance.CloseUI("StageEndSuccessUI");
            LevelManager.Instance.LevelToNext();
        }
    }

    public void LoadData()
    {
        txt_ap.text = NumberToChinese(Game.Instance.GetTotalCostPts());
        if (!Game.Instance.isTest)
        {
            img_map.sprite = maps[int.Parse(SceneManager.GetActiveScene().name[4].ToString()) - 1].sprite;
        }
        buttonStates = Game.Instance.GetMainItemButtonState();
        LoadSkill();
    }

    /// <summary>
    /// 加载技能信息
    /// </summary>
    public void LoadSkill()
    {
        SkillInstanceBase mainSkill = Game.Instance.GetMainSkill();
        Debug.Log(mainSkill);
        List<SkillInstanceBase> items = Game.Instance.GetMainItems();

        for (int i = 0; i < items.Count; i++)
        {
            InstantiateButton(items[i], slotsTrans[i], i);
        }

        if (mainSkill != null)
            InstantiateButton(mainSkill, slotsTrans[3], 3);
    }

    /// <summary>
    /// 实例化按钮，并根据状态信息设置状态
    /// </summary>
    /// <param name="skillInstance"></param>
    /// <param name="trans"></param>
    /// <param name="index"></param>
    private void InstantiateButton(SkillInstanceBase skillInstance, Transform trans, int index)
    {
        if (skillInstance != null)
        {
            GameObject buttonItem = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>(buttonPath + skillInstance.SkillName, trans.position);
            MSkillButton button = buttonItem.transform.GetChild(0).GetChild(0).GetComponent<MSkillButton>();
            buttonItem.transform.SetParent(trans);
            buttonItem.transform.localScale = Vector3.one;
            button.SetSelectMeun(this);
            button.defaultState = buttonStates[index];
            button.SkBtnState = buttonStates[index];
        }

    }


    /// <summary>
    /// 数字转中文
    /// </summary>
    /// <param name="number">eg: 22</param>
    /// <returns></returns>
    public string NumberToChinese(int number)
    {
        if (number == 10)
        {
            return "拾";
        }
        string res = string.Empty;
        string str = number.ToString();
        string schar = str.Substring(0, 1);
        switch (schar)
        {
            case "1":
                res = "壹";
                break;
            case "2":
                res = "贰";
                break;
            case "3":
                res = "叁";
                break;
            case "4":
                res = "肆";
                break;
            case "5":
                res = "伍";
                break;
            case "6":
                res = "陆";
                break;
            case "7":
                res = "柒";
                break;
            case "8":
                res = "捌";
                break;
            case "9":
                res = "玖";
                break;
            default:
                res = "零";
                break;
        }
        if (str.Length > 1 )
        {
            if (int.Parse(str[1].ToString()) == 0)
            {
                res += "拾";
                return res;
            }
            else
                res += NumberToChinese(int.Parse(str.Substring(1, str.Length - 1)));
        }
        return res;
    }

    public void Select(MSkillButton mSkillButton)
    {        
    }

    public void SelectCancel()
    {
    }
}
