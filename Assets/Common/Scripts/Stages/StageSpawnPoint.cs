using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public enum StageMode
{
    LevelMode,
    FreeMode
}

public class StageSpawnPoint : MonoBehaviour
{
    /// <summary>
    /// 存放关卡生成点的字典
    /// </summary>
    public static Dictionary<string, GameObject> SpawnPoint = new Dictionary<string, GameObject>();

    public StageMode stageMode;

    /// <summary>
    /// 将要在此处加载的关卡
    /// </summary>
    public string StageToLoad;

    public CinemachineVirtualCamera levelCamera;
    /// <summary>
    /// 该关卡涉及到的触发器
    /// </summary>
    public GameObject Triggers;
    private GameObject _triggers;

    private void OnEnable()
    {
        SpawnPoint.Add(StageToLoad, gameObject);
    }

    private void OnDisable()
    {
        SpawnPoint.Remove(StageToLoad);
    }

    /// <summary>
    /// 初始化与关卡有关的物品
    /// </summary>
    public void Initialize()
    {
        Debug.Log("Stage Initialized");
        if (Triggers != null)
            _triggers = Instantiate(Triggers, Vector3.zero, Quaternion.identity);
        if (levelCamera != null)
        {
            levelCamera.m_Follow = Game.Instance.GetPlayerUnit().transform;
            CameraChanger.Instance.SetLevelCamera(levelCamera);
        }
        levelCamera?.gameObject.SetActive(true);
    }


    public void Release()
    {
        if (_triggers != null)
            Destroy(_triggers);
        levelCamera?.gameObject.SetActive(false);
        CameraChanger.Instance.SetLevelCamera(null);
    }

    public void LoadStageOnMain()
    {
        switch (stageMode)
        {
            case StageMode.LevelMode:
                Game.Instance.ResetStage();
                Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, StageToLoad);
                break;
            case StageMode.FreeMode:
                Game.Instance.ResetStage();
                PlayerSpawner.Instance.SpawnPlayer(transform);
                Game.Instance.SetCanFreeMove(true);
                Game.Instance.SetCanInput(true);
                //开始更新角色寻路网格
                void action() { Game.Instance.GetPlayerUnit().gameObject.GetComponent<LocalNavMeshBuilder>().StartUpdateNavMesh(); }
                CoroutineManager.StartCoroutineTask(action, 0.2f);
                break;
            default:
                break;
        }
    }

}
