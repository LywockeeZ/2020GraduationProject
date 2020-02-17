//UI窗体（位置）类型
public enum UIFormType
{
    /// <summary>
    /// 普通窗体
    /// </summary>
    Normal,   

    /// <summary>
    /// 固定窗体
    /// </summary>
    Fixed,

    /// <summary>
    /// 弹出窗体
    /// </summary>
    PopUp
}

//UI窗体的显示类型
public enum UIFormShowMode
{
    /// <summary>
    /// 普通，允许多个窗体同时显示
    /// </summary>
    Normal,

    /// <summary>
    /// 反向切换（用于弹出窗体）
    /// </summary>
    ReverseChange,

    /// <summary>
    /// 隐藏其他
    /// </summary>
    HideOther
}

//UI窗体透明度类型
public enum UIFormLucencyType
{
    /// <summary>
    /// 完全透明，不能穿透
    /// </summary>
    Lucency,

    /// <summary>
    /// 半透明，不能穿透
    /// </summary>
    Translucence,

    /// <summary>
    /// 低透明度，不能穿透
    /// </summary>
    ImPenetrable,

    /// <summary>
    /// 可以穿透
    /// </summary>
    Pentrate
}

