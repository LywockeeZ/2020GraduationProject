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
            default:
                break;
        }

        return skillInstance;
    }
}
