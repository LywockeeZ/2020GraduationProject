using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class BattleUI :BaseUIForm
{
    public GameObject ActionBar;
    public GameObject resetBtn;
    public GameObject warmingBtn;
    public GameObject endBtn;

    private Image apBar;
    private Image roundTag;
    private GameObject skillUI = null;

    private Tweener tweener1;
    private Tweener tweener2;
    private Tweener tweener3;
    private Tweener tweener4;



    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.Normal;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Pentrate;

        apBar = UITool.GetUIComponent<Image>(ActionBar, "bar_Ap");
        roundTag = UITool.GetUIComponent<Image>(ActionBar, "tag_Round");

        tweener1 = ActionBar.transform.DOLocalMoveY(ActionBar.transform.localPosition.y - 140, 1f).SetEase(Ease.OutCubic).SetAutoKill(false);
        tweener2 = resetBtn.transform.DOLocalMoveY(resetBtn.transform.localPosition.y - 70, 0.5f).SetEase(Ease.OutCubic).SetAutoKill(false);
        tweener3 = warmingBtn.transform.DOLocalMoveY(warmingBtn.transform.localPosition.y - 70, 0.5f).SetEase(Ease.OutCubic).SetAutoKill(false);
        tweener4 = endBtn.transform.DOLocalMoveY(endBtn.transform.localPosition.y - 70, 0.5f).SetEase(Ease.OutCubic).SetAutoKill(false);

        tweener1.Pause();
        tweener2.Pause();
        tweener3.Pause();
        tweener4.Pause();

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
        

    private void OnEnable()
    {
        RegisterEvent();

        Game.Instance.ShowUI("SkillBarUI");
        tweener1.Restart();
        tweener2.Restart();
        tweener3.Restart();
        tweener4.Restart();
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
        Game.Instance.NotifyEvent(ENUM_GameEvent.RoundUpdateBegain);
    }

    public void BtnChangeCam()
    {
        CameraChanger.Instance.ChangeCam();
    }

    public void BtnExit()
    {
        if (Game.Instance.GetIsInStage())
        {
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageEnd);
        }
        Game.Instance.CloseAll();
        Game.Instance.LoadLevel("StartScene");
    }

    public void BtnReset()
    {
        Game.Instance.CloseUI("BattleUI");
        Game.Instance.CloseUI("SkillBarUI");
        if (Game.Instance.isTest)
        {
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageEnd, 4);
        }
        else
        {
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageEnd, 4);
            //LevelManager.Instance.BackToLevel();
        }
    }

    private void OnDisable()
    {
        DetachEvent();
    }

}
