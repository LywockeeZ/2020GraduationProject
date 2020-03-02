using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISkillFactory 
{
    public abstract SkillInstanceBase GetSkillInstance(string skillName);
}
