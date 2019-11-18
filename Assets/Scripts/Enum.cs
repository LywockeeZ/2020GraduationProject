
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
    {
        RoundBegain,
        RoundEnd,

        RoundUpdateBegain,
        FireUpdate,
        RoundUpdateEnd,

        StageBegain,
        StageEnd,
    }

    public enum ENUM_Build_BaseUnit
    {
        Null,
        Ground,
        Fire,
        Water,
        Oil,
        Block
    }

    public enum ENUM_Build_UpperUnit
    {
        Null,
        Chest,
        RoadBlock,
        OilTank,
        WaterTank,
        Player,
        Survivor
    }


