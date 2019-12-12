using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EventTriger : MonoBehaviour
{
    public ENUM_GameEvent GameEvent;
    public string EventParam;
    public GameObject Object;
    public Text content;

    private void Start()
    {
    }

    //private EventListenerDelegate OnTeachingUIOn;
    //private EventListenerDelegate OnTeachingUIOff;
    //private void RegisterEvent()
    //{
    //    Game.Instance.RegisterEvent(ENUM_GameEvent.TeachingUIOn,
    //    OnTeachingUIOn = (Message evt) =>
    //    {
    //        Game.Instance.SetCanInput(false);
    //    });

    //    Game.Instance.RegisterEvent(ENUM_GameEvent.TeachingUIOff,
    //    OnTeachingUIOff = (Message evt) =>
    //    {
    //        Game.Instance.SetCanInput(true);
    //    });

    //}

    private void DetachEvent()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        content.text = EventParam;
        if (Game.Instance.GetCanFreeMove())
        {
            other.transform.gameObject.GetComponent<ClickToMove>().enabled = false;
            other.transform.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
        else Game.Instance.NotifyEvent(ENUM_GameEvent.StageEnd, null);
        GUIManager.Instance.CloseAll();
        other.transform.position = transform.position;
        Object.SetActive(true);
        Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, null);
        Destroy(gameObject);
    }

}
