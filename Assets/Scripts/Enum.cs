using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enum 
{
    //状态
    public enum ENUM_State
    {
        Ground,
        HotFire,
        Fire,
        Water,
        Oil,
        Block
    }

    //输入事件
    public enum ENUM_InputEvent
    {
        Up,
        Down,
        Left,
        Right
    }


    public enum ENUM_UpperUnitType
    {
        NULL,
        Movable,
        Fixed
    }

    //游戏事件
    public enum ENUM_GameEvent
    { }

}
