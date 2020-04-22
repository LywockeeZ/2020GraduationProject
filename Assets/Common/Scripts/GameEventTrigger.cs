using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameEventTrigger : MonoBehaviour
{
    public bool dontDestroyOnTouch = false;
    public UnityEvent OnTriggerEvent;


    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEvent.Invoke();
        if (!dontDestroyOnTouch)
        {
            Destroy(gameObject);
        }
    }
}
