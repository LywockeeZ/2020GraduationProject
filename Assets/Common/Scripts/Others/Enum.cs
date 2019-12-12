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

    /// <summary>
    /// 用来区分火焰是直接点燃还是触发操作
    /// </summary>
    public enum ENUM_StateBeFiredType
    {
        False,
        BeFire,
        BeHandle,
    }

    //输入事件
    public enum ENUM_InputEvent
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum ENUM_UpperUnit
    {
        NULL,
        Chest,
        RoadBlock,
        OilTank,
        WaterTank,
        Player,
        Survivor
    }

    /// <summary>
    /// 区分玩家对上层单元的操控类型
    /// </summary>
    public enum ENUM_UpperUnitControlType
    {
        NULL,
        Movable,
        Fixed
    }

    /// <summary>
    /// 区分上层单元是否可以被火焰触发操作
    /// </summary>
    public enum ENUM_UpperUnitBeFiredType
    {
        NULL,
        BeFire
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
        StageRestart,

        LoadSceneStart,
        LoadSceneComplete,

        TeachingUIOn,
        TeachingUIOff
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


