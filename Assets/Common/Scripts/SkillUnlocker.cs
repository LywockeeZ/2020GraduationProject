using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class SkillUnlocker : MonoBehaviour
{
    public void UnlockSkill(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("SkillName不能为空");
        }
        else
        {
            Dictionary<string, SkillInstanceBase> unlockskills = Game.Instance.GetUnlockSkills();
            SkillInstanceBase skill = null;
            unlockskills.TryGetValue(name, out skill);
            if (skill == null)
            {
                Game.Instance.UnlockSkill(name);
                Flowchart.BroadcastFungusMessage(name);
            }

        }
    }

}
