using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameEventTrigger : MonoBehaviour
{
    public UnityEvent OnTriggerEvent;

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void OnDisable()
    {
        DetachEvent();
    }

    private EventListenerDelegate OnStageRestart;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageRestart,
            OnStageRestart = (Message evt) =>
            {
                gameObject.GetComponent<BoxCollider>().enabled = true;
            });
    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.StageRestart, OnStageRestart);
    }



    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEvent.Invoke();
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }
}
