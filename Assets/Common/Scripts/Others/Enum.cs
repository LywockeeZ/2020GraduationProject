/// <summary>
/// 下层类型的状态
/// </summary>
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
/// 用来区分火焰对下层单元状态的操作，是直接点燃还是触发操作
/// </summary>
public enum ENUM_StateBeFiredType
{
    /// <summary>
    /// 不能点燃
    /// </summary>
    False,
    /// <summary>
    /// 可以被火焰点燃
    /// </summary>
    BeFire,
    /// <summary>
    /// 可以被火焰触发操作
    /// </summary>
    BeHandle,
}

/// <summary>
/// 输入事件
/// </summary>
public enum ENUM_InputEvent
{
    Up,
    Down,
    Left,
    Right
}

/// <summary>
/// 上层单元类型
/// </summary>
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
    /// <summary>
    /// 无法操控
    /// </summary>
    NULL,
    /// <summary>
    /// 可以移动
    /// </summary>
    Movable,
    /// <summary>
    /// 固定触发
    /// </summary>
    Fixed
}

/// <summary>
/// 区分上层单元是否可以被火焰触发操作
/// </summary>
public enum ENUM_UpperUnitBeFiredType
{
    /// <summary>
    /// 无法触发
    /// </summary>
    NULL,
    /// <summary>
    /// 可以被点燃
    /// </summary>
    BeFire
}

/// <summary>
/// 游戏主要事件
/// </summary>
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

    GamePause,

    StartFire
}


/// <summary>
/// 建造时使用的下层类型
/// </summary>
public enum ENUM_Build_BaseUnit
{
    Null,
    Ground,
    Fire,
    Water,
    Oil,
    Block
}

/// <summary>
/// 建造时使用的上层类型
/// </summary>
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

/// <summary>
/// 关卡的类型
/// </summary>
public enum ENUM_StageType
{
    /// <summary>
    /// 普通的关卡
    /// </summary>
    Normal,
    /// <summary>
    /// 教学关卡
    /// </summary>
    Teach,
    /// <summary>
    /// 不进入关卡链的普通关卡
    /// </summary>
    NormalSingle,
    /// <summary>
    /// 不进入关卡链的教学关卡
    /// </summary>
    TeachSingle
}


