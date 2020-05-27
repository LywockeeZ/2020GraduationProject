using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class SkillUI : BaseUIForm
{
    private GameObject SkillParant;
    private GameObject ItemParent;
    private MMTouchButton skillBtn;

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.Normal;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Pentrate;

        SkillParant = UnityTool.FindChildGameObject(gameObject, "Skill");
        ItemParent = UnityTool.FindChildGameObject(gameObject, "Items");

    }

    private void Start()
    {
        GameObject mainSkill = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>(
            "UI/UIComponent/SkillMainIcon/" + Game.Instance.GetMainSkill().SkillName, Vector3.zero);
        mainSkill.transform.SetParent(SkillParant.transform);
        mainSkill.transform.localPosition = Vector3.zero;
        mainSkill.transform.localScale = Vector3.one;
        skillBtn = UITool.GetUIComponent<MMTouchButton>(mainSkill, "Background");
        void ExecuteSkill()
        {
            Game.Instance.GetPlayerUnit().ExecuteSkill();
            skillBtn.DisableButton();
        }
        skillBtn.ButtonPressedFirstTime.AddListener(ExecuteSkill);
        
    }

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void OnDisable()
    {
        DetachEvent();
    }

    private void OnDestroy()
    {
        DetachEvent();
    }

    private EventListenerDelegate OnRoundBegain;
    private EventListenerDelegate OnRoundUpdateBegain;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundBegain,
            OnRoundBegain = (Message evt) =>
            {
                //skillBtn.EnableButton();
            });
        Game.Instance.RegisterEvent(ENUM_GameEvent.RoundUpdateBegain,
            OnRoundUpdateBegain = (Message evt) =>
            {
                //skillBtn.DisableButton();
            });

    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundBegain, OnRoundBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.RoundUpdateBegain, OnRoundUpdateBegain);

    }
}
