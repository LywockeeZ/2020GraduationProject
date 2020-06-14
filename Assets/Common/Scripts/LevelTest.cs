using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 作用：单个场景测试的启动入口，前提是GameTest已经运行
/// </summary>
public class LevelTest : MonoBehaviour
{
    /// <summary>
    /// 测试UI上文本框显示的信息
    /// </summary>
    public string StartMessage;

    public bool isStartScene;
    /// <summary>
    /// 当关卡测试时，要加载的关卡名
    /// </summary>
    public string LevelName;

    private void Awake()
    {
        if (GameObject.Find("GameTest") == null)
        {
            GameObject gameTest = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Prefabs/Others/GameTest", Vector3.zero);
            gameTest.name = "GameTest";
        }
    }

    void Start()
    {
        Game.Instance.isTest = true;

        if (!isStartScene)
        {
            //开始界面加载而来的关卡，开始前显示关卡名
            if (SceneManager.GetActiveScene().name == "NewStage")
            {
                Game.Instance.UnlockAllSkill();
                StartMessage = GameTest.Instance.CurrentStage;
                Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, GameTest.Instance.CurrentStage);
            }
            else
            {
                if (!string.IsNullOrEmpty(LevelName))
                {
                    Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, LevelName);
                }
                else Debug.Log("关卡名不能为空");
            }
        }
        else
            //如果测试的是选关界面，打开选关UI
            Game.Instance.ShowUI("TestLevelSelectUI");


        if (!string.IsNullOrEmpty(StartMessage) && !isStartScene)
        {
            //Game.Instance.UIShowMessag("TestStartUI", StartMessage);
        }

    }

    private void Update()
    {
        //Debug.Log(Game.Instance.GetPlayerUnit().CurrentOn.State.Model.transform.position);
    }

    public void OnDestroy()
    {
        Game.Instance.isTest = false;
    }

}
