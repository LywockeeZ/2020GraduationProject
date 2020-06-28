using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

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

    public void CloseUI()
    {
        void action()
        {
            Game.Instance.CloseUI(UIName);
        }
        CoroutineManager.StartCoroutineTask(action, delay);
    }

    public void ShowFungusUI()
    {
        void action()
        {
            Flowchart.BroadcastFungusMessage(UIName);
        }
        CoroutineManager.StartCoroutineTask(action, delay);
    }

    public void PlayerMoveBack()
    {
        Game.Instance.GetPlayerUnit().MoveByNavMesh(Game.Instance.GetPlayerUnit().transform.position - 2 * Game.Instance.GetPlayerUnit().transform.forward, false);
    }
}
