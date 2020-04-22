using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using Cinemachine;
using DG.Tweening;
using HighlightingSystem;

public class CharacterTalk : MonoBehaviour
{
    public GameObject trigger1;
    public CinemachineVirtualCamera NpcCam;
    public string message;

    private void Start()
    {
        if (trigger1 != null)
        {
            trigger1.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Game.Instance.SetCanInput(false);
        Game.Instance.SetCanFreeMove(false);
        Flowchart.BroadcastFungusMessage("Character1");
        trigger1.SetActive(true);
    }

    public void OnTalkEnd()
    {
        NpcCam?.VirtualCameraGameObject.SetActive(false);
        Game.Instance.SetCanFreeMove(true);
        Game.Instance.SetCanInput(true);
    }

    public void Talk()
    {
        if (trigger1 != null)
        {
            trigger1.SetActive(true);
        }
        NpcCam?.VirtualCameraGameObject.SetActive(true);
        Game.Instance.SetCanInput(false);
        Game.Instance.SetCanFreeMove(false);
        Game.Instance.GetPlayerUnit().MoveByNavMesh(Game.Instance.GetPlayerUnit().transform.position);
        Game.Instance.GetPlayerUnit().transform.DOLookAt(transform.position, 1f);
        Game.Instance.GetPlayerUnit().MoveByNavMesh((Game.Instance.GetPlayerUnit().transform.position-transform.position).normalized*1.5f + transform.position);
        Flowchart.BroadcastFungusMessage(message);
    }

    public void InputOff()
    {
        Game.Instance.SetCanInput(false);
    }

    public void InputOn()
    {
        Game.Instance.SetCanInput(true);
    }


}
