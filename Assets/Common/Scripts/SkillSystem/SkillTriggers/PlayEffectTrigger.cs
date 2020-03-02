using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectTrigger : AbstractSkillTrigger
{
    public override ISkillTrigger Clone()
    {
        throw new System.NotImplementedException();
    }

    public override bool Execute(ISkillCore instance)
    {
        return true;
    }

    public override void Init()
    {
        
    }
}
