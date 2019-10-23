using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void mydelegate(Enum.ENUM_InputEvent inputEvent);
    public static event mydelegate InputEvent;


    void Start()
    {
        
    }


    void Update()
    {
        if (GameManager.Instance.canInput)
        {
            InputProcess();
        }
    }

    private void InputProcess()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))   InputEvent(Enum.ENUM_InputEvent.Right);
        else 
        if (Input.GetKeyDown(KeyCode.LeftArrow))   InputEvent(Enum.ENUM_InputEvent.Left);
        else
        if (Input.GetKeyDown(KeyCode.UpArrow))  InputEvent(Enum.ENUM_InputEvent.Up);
        else
        if (Input.GetKeyDown(KeyCode.DownArrow))    InputEvent(Enum.ENUM_InputEvent.Down);

    }
}
