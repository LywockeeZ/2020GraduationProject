using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrigger : MonoBehaviour
{
    public string UIName;
    public float delay = 0;

    public void ShowUI()
    {
        void action()
        {
            Game.Instance.ShowUI(UIName);
        }
        CoroutineManager.StartCoroutineTask(action, delay);
    }
}
