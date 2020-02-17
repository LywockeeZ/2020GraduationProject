﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleUI :BaseUIForm
{
    public Button btn_Reset;
    public Button btn_Warming;

    public GameObject ActionBar;
    public Image apBar;
    public Image roundTag;


    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.Normal;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.HideOther;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Pentrate;

        btn_Reset = UITool.GetUIComponent<Button>(gameObject, "btn_Reset");
        btn_Warming = UITool.GetUIComponent<Button>(gameObject, "btn_Warming");
        ActionBar = UnityTool.FindChildGameObject(gameObject, "ActionBar");
        apBar = UITool.GetUIComponent<Image>(ActionBar, "bar_Ap");
        roundTag = UITool.GetUIComponent<Image>(ActionBar, "tag_Round");

    }

    private EventListenerDelegate OnStageEnd;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageEnd,
        OnStageEnd = (Message evt) =>
        {
            Game.Instance.CloseUI("BattleUI");
        });

    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.StageEnd, OnStageEnd);
    }
        

    private void Start()
    {

        btn_Reset.onClick.AddListener(() => {
            Game.Instance.CloseUI("BattleUI");
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageRestart);
            //Game.Instance.LoadLevel(SceneManager.GetActiveScene().name);
        });
    }

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void Update()
    {
        
    }

    public void SetActionBar(int ap)
    {
        apBar.sprite = GameFactory.GetAssetFactory().LoadAsset<Sprite>("Images/UI/ap/ap_" + ap.ToString());
    }

    public void SetRoundTag(int round)
    {
        if (round > 10)
            return;

        roundTag.sprite = GameFactory.GetAssetFactory().LoadAsset<Sprite>("Images/UI/round/round_" + round.ToString());
    }

    public void BtnEndRound()
    {
        Game.Instance.NotifyEvent(ENUM_GameEvent.RoundEnd);
    }

    private void OnDisable()
    {
        DetachEvent();
    }

}