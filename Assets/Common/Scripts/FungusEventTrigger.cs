using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class FungusEventTrigger : MonoBehaviour
{
    public string message;
    public float delay = 0;

    public void SendMessage()
    {
        void action()
        {
            Flowchart.BroadcastFungusMessage(message);
        }
        CoroutineManager.StartCoroutineTask(action, delay);
    }
}
