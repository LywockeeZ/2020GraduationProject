using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject Player;

    private void Awake()
    {
        Debug.Log("Player has been spawned");
        Player = GameFactory.GetAssetFactory().InstantiateGameObject("Player", transform.position);
        Player.GetComponent<NavMeshAgent>().updatePosition = true;
    }

    private EventListenerDelegate OnLoadSceneStart;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.LoadSceneStart,
            OnLoadSceneStart = (Message evt) =>
            {
                if (Game.Instance.GetCurrentStage() == null)
                {
                    GameFactory.GetAssetFactory().DestroyGameObject<IUpperUnit>(Player);
                }
                else Game.Instance.GetCurrentStage().Reset();
                Game.Instance.SetCanFreeMove(false);
            });
    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.LoadSceneStart, OnLoadSceneStart);
    }

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void OnDisable()
    {
        DetachEvent();
    }
}
