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
    public bool isFreeModeTest;

    private void Awake()
    {
        if (GameObject.Find("GameTest") == null)
        {
            GameObject gameTest = new GameObject("GameTest");
            gameTest.AddComponent<GameTest>();
        }
    }

    void Start()
    {
        Game.Instance.isTest = true;


        if (!isStartScene && !isFreeModeTest)
        {
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, GameTest.Instance.CurrentStage);
            //开始界面加载而来的关卡，开始前显示关卡名
            if (SceneManager.GetActiveScene().name == "NewStage")
            {
                StartMessage = GameTest.Instance.CurrentStage;
            }

        }
        else
        if (isStartScene)
        {
            //如果测试的是选关界面，打开选关UI
            Game.Instance.ShowUI("TestLevelSelectUI");
        }
        else
        {
            //如果测试的是自由模式，设置鼠标输入为自由模式的输入
            Game.Instance.SetCanFreeMove(true);
            void action(){ Game.Instance.GetPlayerUnit().gameObject.GetComponent<LocalNavMeshBuilder>().StartUpdateNavMesh(); }
            CoroutineManager.StartCoroutineTask(action, 0.5f);
            
        }


        if (!string.IsNullOrEmpty(StartMessage) && !isStartScene)
        {
            Game.Instance.UIShowMessag("TestStartUI", StartMessage);
        }


    }

    private void Update()
    {
        //Debug.Log(Game.Instance.GetPlayerUnit().CurrentOn.State.Model.transform.position);
    }

}
