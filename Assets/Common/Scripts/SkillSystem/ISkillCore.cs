using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillCore 
{
     Animator SkillAnimator { get; }
     IUpperUnit UpperUnit { get; }
    void ExecuteSkill();
}
