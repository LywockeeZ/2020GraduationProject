using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

public class TestLevelSelectUI : BaseUIForm
{
    private void Awake()
    {
        CurrentUIType.UIForm_Type = UIFormType.Normal;
        CurrentUIType.UIForm_ShowMode = UIFormShowMode.HideOther;
        CurrentUIType.UIForm_LucencyType = UIFormLucencyType.Lucency;
    }

    void Start()
    {
        GameObject StageItem;
        Text itemText;
        MMTouchButton mMTouchButton;
        LevelSelector selector;
        GameObject carousel = UnityTool.FindChildGameObject(gameObject, "MMCarousel");
        GameObject carouselContent = UnityTool.FindChildGameObject(carousel, "Content");
        Dictionary<string, IStageHandler> dicAllStages = Game.Instance.GetStages();
        foreach (KeyValuePair<string, IStageHandler> stage in dicAllStages)
        {
            StageItem = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>(
                "UI/UIComponent/StageItem", Vector3.zero);
            StageItem.name = stage.Key + "Test";
            itemText = UITool.GetUIComponent<Text>(StageItem, "Text");
            itemText.text = stage.Key;
            selector = UnityTool.FindChildGameObject(StageItem, "Background").GetComponent<LevelSelector>();
            selector.levelName = stage.Key;
            mMTouchButton = UITool.GetUIComponent<MMTouchButton>(StageItem, "Background");
            mMTouchButton.ButtonPressedFirstTime.AddListener(() => { Close(); });
            StageItem.transform.SetParent(carouselContent.transform);
            StageItem.transform.localScale = new Vector3(1, 1, 1);
        }
    }


    public void Close()
    {
        Game.Instance.CloseUI("TestLevelSelectUI");
    }

}
