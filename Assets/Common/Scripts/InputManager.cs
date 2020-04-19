﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

public class InputManager : MonoBehaviour
{
    public delegate void mydelegate(ENUM_InputEvent inputEvent);
    public static event mydelegate InputEvent;


    void Start()
    {

    }


    void Update()
    {
        if (Game.Instance.GetCanInput())
        {
            InputProcess();
        }
    }

    private void InputProcess()
    {
        MouseInput();
        if (!Game.Instance.GetCanFreeMove())
        {
            KeyboardInput();
        }
    }


    private void MouseInput()
    {
        HighlightTarget();

        //鼠标操作,按键弹起时进入
        if (Input.GetMouseButtonUp(0) == false)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("BaseUnit")))
        {
            Vector3 clickedPos;

            //关卡模式中
            if (!Game.Instance.GetCanFreeMove())
            {
                clickedPos = hitInfo.transform.position;
                if (Game.Instance.GetPlayerUnit().CurrentOn.Up != null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Up.Model.transform.position)
                    {
                        InputEvent(ENUM_InputEvent.Up);
                        return;
                    }
                }
                if (Game.Instance.GetPlayerUnit().CurrentOn.Down != null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Down.Model.transform.position)
                    {
                        InputEvent(ENUM_InputEvent.Down);
                        return;
                    }
                }
                if (Game.Instance.GetPlayerUnit().CurrentOn.Left != null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Left.Model.transform.position)
                    {
                        InputEvent(ENUM_InputEvent.Left);
                        return;
                    }
                }
                if (Game.Instance.GetPlayerUnit().CurrentOn.Right != null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Right.Model.transform.position)
                    {
                        InputEvent(ENUM_InputEvent.Right);
                        return;
                    }
                }

            }

            if (Physics.Raycast(ray2, out RaycastHit hitInfo1, 200f, 1 << LayerMask.NameToLayer("CanMove")))
            {
                //自由移动模式中的移动
                if (Game.Instance.GetCanFreeMove())
                {
                    clickedPos = hitInfo.point;
                    Game.Instance.GetPlayerUnit().MoveByNavMesh(clickedPos);
                    Debug.Log(clickedPos);
                }
            }

        }
    }

    private void KeyboardInput()
    {
        //键盘操作
        if (Input.GetKeyDown(KeyCode.RightArrow)) InputEvent(ENUM_InputEvent.Right);
        else
        if (Input.GetKeyDown(KeyCode.LeftArrow)) InputEvent(ENUM_InputEvent.Left);
        else
        if (Input.GetKeyDown(KeyCode.UpArrow)) InputEvent(ENUM_InputEvent.Up);
        else
        if (Input.GetKeyDown(KeyCode.DownArrow)) InputEvent(ENUM_InputEvent.Down);

    }

    private void HighlightTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Game.Instance.GetCanFreeMove())
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("BaseUnit")))
            {
                Vector3 hoverPos = hitInfo.transform.position;
                if (hoverPos == Game.Instance.GetPlayerUnit().CurrentOn.Right?.Model.transform.position ||
                    hoverPos == Game.Instance.GetPlayerUnit().CurrentOn.Left?.Model.transform.position ||
                    hoverPos == Game.Instance.GetPlayerUnit().CurrentOn.Up?.Model.transform.position ||
                    hoverPos == Game.Instance.GetPlayerUnit().CurrentOn.Down?.Model.transform.position)
                {
                    Highlighter highlighter = hitInfo.transform.gameObject.GetComponent<Highlighter>();
                    highlighter.Hover(Color.white);
                }
            }
        }
        else
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 20f, 1 << LayerMask.NameToLayer("NPC")))
            {
                if ((hitInfo.transform.position - Game.Instance.GetPlayerUnit().transform.position).magnitude < 5)
                {
                    Vector3 hoverPos = hitInfo.transform.position;
                    Highlighter highlighter = hitInfo.transform.gameObject.GetComponent<Highlighter>();
                    highlighter.Hover(Color.white);

                    if (Input.GetMouseButtonDown(0))
                    {
                        hitInfo.transform.gameObject.GetComponent<CharacterTalk>().Talk();
                        return;
                    }
                }
            }

        }
    }
}
