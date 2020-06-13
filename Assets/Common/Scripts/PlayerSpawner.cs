﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSpawner : Singleton<PlayerSpawner>
{
    public GameObject Player;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SpawnPlayer(Transform targetTrans)
    {
        Debug.Log("Player has been spawned");
        transform.position = targetTrans.position;
        transform.rotation = targetTrans.rotation;
        if (Game.Instance.GetPlayerUnit() == null)
        {
            Player = GameFactory.GetAssetFactory().InstantiateGameObject("Player", transform.position);
            Player.transform.rotation = transform.rotation;
            Player.GetComponent<NavMeshAgent>().updatePosition = true;
            Player.GetComponent<NavMeshAgent>().Warp(targetTrans.position);
        }
        else
        {
            Player = Game.Instance.GetPlayerUnit().gameObject;
            Player.SetActive(true);
            Player.transform.rotation = transform.rotation;
            Player.transform.position = transform.position;
            Player.GetComponent<NavMeshAgent>().updatePosition = true;
            Player.GetComponent<NavMeshAgent>().Warp(targetTrans.position);
        }

    }

    //切换场景时回收游戏对象
    private EventListenerDelegate OnLoadSceneStart;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.LoadSceneStart,
            OnLoadSceneStart = (Message evt) =>
            {
                if (Game.Instance.GetCanFreeMove())
                {
                    GameFactory.GetAssetFactory().DestroyGameObject<IUpperUnit>(Player);
                }
                else Game.Instance.ResetStage();
                Game.Instance.SetCanFreeMove(false);
            });
    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.LoadSceneStart, OnLoadSceneStart);
    }

    private void OnEnable()
    {
        CoroutineManager.StartCoroutineTask(RegisterEvent, 0.5f);
    }

    private void OnDisable()
    {
        DetachEvent();
    }
}
