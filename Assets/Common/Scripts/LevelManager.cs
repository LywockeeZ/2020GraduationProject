using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        LoadLevel(Game.Instance.GetCurrentLevelOnMain());
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
}
