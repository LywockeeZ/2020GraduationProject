using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enum 
{
    //状态的枚举
    public enum ENUM_State
    {
        Ground,
        HotFire,
        Fire,
        Water,
        Boil,
        Block
    }

    //输入事件的枚举类型
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
}
