using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //MouseInput();
        KeyboardInput();
    }


    private void MouseInput()
    {
        //鼠标操作,按键弹起时进入
        if (Input.GetMouseButtonUp(0) == false)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("BaseUnit")))
        {
            Vector3 clickedPos = hitInfo.transform.position;
            if (GameManager.Instance.GetPlayerUnit().CurrentOn.Up != null)
            {
                if (clickedPos == GameManager.Instance.GetPlayerUnit().CurrentOn.Up.Model.transform.position)
                {
                    InputEvent(ENUM_InputEvent.Up);
                    return;
                }
            }
            if (GameManager.Instance.GetPlayerUnit().CurrentOn.Down != null)
            {
                if (clickedPos == GameManager.Instance.GetPlayerUnit().CurrentOn.Down.Model.transform.position)
                {
                    InputEvent(ENUM_InputEvent.Down);
                    return;
                }
            }
            if (GameManager.Instance.GetPlayerUnit().CurrentOn.Left != null)
            {
                if (clickedPos == GameManager.Instance.GetPlayerUnit().CurrentOn.Left.Model.transform.position)
                {
                    InputEvent(ENUM_InputEvent.Left);
                    return;
                }
            }
            if (GameManager.Instance.GetPlayerUnit().CurrentOn.Right != null)
            {
                if (clickedPos == GameManager.Instance.GetPlayerUnit().CurrentOn.Right.Model.transform.position)
                {
                    InputEvent(ENUM_InputEvent.Right);
                    return;
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
}
