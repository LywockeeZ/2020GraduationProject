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
    public bool GoBackOnTalk = true;
    public string TalkAnimation;
    public string message;
    public float highlightDistance = 5f;
    public UnityEvent OnMouseClick;

    private Highlighter highlighter;
    private bool canClick = true;
    private bool isHighlight = false;

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
        if (Game.Instance.GetPlayerUnit()!= null && canClick && Game.Instance.GetCanInput())
        {
            if ((transform.position - Game.Instance.GetPlayerUnit().transform.position).magnitude < highlightDistance)
            {
                highlighter.ConstantOn(highlighter.constantFadeInTime);
                isHighlight = true;
            }
        }
    }

    private void OnMouseOver()
    {
        if (!isHighlight && Game.Instance.GetPlayerUnit() != null && canClick && Game.Instance.GetCanInput())
        {
            if ((transform.position - Game.Instance.GetPlayerUnit().transform.position).magnitude < highlightDistance)
            {
                highlighter.ConstantOn(highlighter.constantFadeInTime);
                isHighlight = true;
            }
        }
    }

    private void OnMouseExit()
    {
        highlighter.ConstantOff(highlighter.constantFadeOutTime);
        isHighlight = false;
    }

    private void OnMouseDown()
    {
        if ((transform.position - Game.Instance.GetPlayerUnit().transform.position).magnitude < highlightDistance && canClick && Game.Instance.GetCanInput())
        {
            highlighter.ConstantOff(highlighter.constantFadeOutTime);
            isHighlight = false;
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
        Game.Instance.GetPlayerUnit().MoveByNavMesh(Game.Instance.GetPlayerUnit().transform.position, false);
        Game.Instance.GetPlayerUnit().transform.DOLookAt(transform.position, 1f);
        if(GoBackOnTalk)
            Game.Instance.GetPlayerUnit().MoveByNavMesh((Game.Instance.GetPlayerUnit().transform.position-transform.position).normalized*2f + transform.position, false);
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
