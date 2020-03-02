
public enum SkillType
{
    None = -1,
    /// <summary>
    /// 物品本质上也是一种技能
    /// </summary>
    Item = 0,
    /// <summary>
    /// 常规技能
    /// </summary>
    NormalSkill
}

public enum SkillTriggerState
{
    Playing,
    End
}

public enum SkillState
{
    Playing,
    End
}

public enum SkillTriggerType
{
    Animation,
    Effect
}