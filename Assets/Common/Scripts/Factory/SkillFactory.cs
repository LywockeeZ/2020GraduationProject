using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFactory : ISkillFactory
{
    public override SkillInstanceBase GetSkillInstance(string skillName)
    {
        SkillInstanceBase skillInstance = null;
        switch (skillName)
        {
            case "Whirlwind":
                skillInstance = new Whirlwind(skillName);
                break;
            case "NormalAttack":
                skillInstance = new NormalAttack(skillName);
                break;
            default:
                Debug.LogError("未找到技能" + skillName);
                break;
        }

        return skillInstance;
    }
}
