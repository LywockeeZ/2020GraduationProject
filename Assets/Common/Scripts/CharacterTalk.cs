using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fungus;
using Cinemachine;
using DG.Tweening;
using HighlightingSystem;

public class CharacterTalk : MonoBehaviour
{
    public GameObject trigger1;
    public CinemachineVirtualCamera NpcCam;
    public Animator animator;
    public string TalkAnimation;
    public string message;
    public float highlightDistance = 5f;
    public UnityEvent OnMouseClick;

    private Highlighter highlighter;
    private bool canClick = true;

    private void Start()
    {
        highlighter = GetComponent<Highlighter>();
        if (trigger1 != null)
        {
            trigger1.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        if ((transform.position - Game.Instance.GetPlayerUnit().transform.position).magnitude < highlightDistance && canClick && Game.Instance.GetCanInput())
        {
            highlighter.ConstantOn(highlighter.constantFadeInTime);
        }
    }

    private void OnMouseExit()
    {
        highlighter.ConstantOff(highlighter.constantFadeOutTime);
    }

    private void OnMouseDown()
    {
        if ((transform.position - Game.Instance.GetPlayerUnit().transform.position).magnitude < highlightDistance && canClick && Game.Instance.GetCanInput())
        {
            highlighter.ConstantOff(highlighter.constantFadeOutTime);
            OnMouseClick?.Invoke();
            if (animator != null && !string.IsNullOrEmpty(TalkAnimation))
            {
                animator.SetTrigger(TalkAnimation);
            }
        }
    }


    public void OnTalkEnd()
    {
        canClick = true;
        NpcCam?.VirtualCameraGameObject.SetActive(false);
        Game.Instance.SetCanFreeMove(true);
        Game.Instance.SetCanInput(true);
    }

    public void Talk()
    {
        canClick = false;
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
