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
            case "skill_Whirlwind":
                skillInstance = new Whirlwind(skillName);
                break;
            case "skill_NormalAttack":
                skillInstance = new NormalAttack(skillName);
                break;
            case "skill_Slash":
                skillInstance = new Slash(skillName);
                break;
            case "item_WaterSac":
                skillInstance = new WaterSac(skillName);
                break;
            default:
                Debug.LogError("未找到技能" + skillName);
                break;
        }

        return skillInstance;
    }
}
