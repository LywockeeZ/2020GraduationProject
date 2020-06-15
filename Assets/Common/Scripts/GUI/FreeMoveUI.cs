using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class FreeMoveUI : BaseUIForm
{
    public GameObject endBtn;

    private Tweener tweener1;

    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.Normal;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Pentrate;

        tweener1 = endBtn.transform.DOLocalMoveY(endBtn.transform.localPosition.y - 70, 0.5f).SetEase(Ease.OutCubic).SetAutoKill(false);
        tweener1.Pause();

    }

    private void OnEnable()
    {
        tweener1.Restart();
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


}
