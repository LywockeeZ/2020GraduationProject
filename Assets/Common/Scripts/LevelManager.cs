using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Level to load")]
    public string LevelName;
    [Header("Level to back")]
    public string SceneName;
    public string PointName;
    [Header("Level to next")]
    public string NextSceneName;
    public string NextPointName;

    private string levelToLoad;
    

    protected override void Awake()
    {
        base.Awake();
        if (GameObject.Find("GameTest") == null)
        {
            GameObject gameTest = new GameObject("GameTest");
            gameTest.AddComponent<GameTest>();
        }

    }

    private void Start()
    {
        //接收并加载需要加载的关卡
        levelToLoad = Game.Instance.GetLevelWillToOnMain();
        if (!string.IsNullOrEmpty(levelToLoad))
        {
            LoadLevel(levelToLoad);
        }
        else
            LoadLevel(LevelName);
    }


    public void LoadLevel(string levelName)
    {
        if (!string.IsNullOrEmpty(levelName))
        {
            GameObject spawnPoint;
            StageSpawnPoint.SpawnPoint.TryGetValue(levelName, out spawnPoint);
            spawnPoint.GetComponent<StageSpawnPoint>().LoadStageOnMain();
        }
    }

    /// <summary>
    /// 关卡中，重置关卡按钮所用
    /// </summary>
    public void BackToLevel()
    {
        Game.Instance.LoadLevelOnMain(SceneName, PointName);
    }

    public void LevelToNext()
    {
        Game.Instance.LoadLevelOnMain(NextSceneName, NextPointName);
    }
}
