using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class SkillUnlocker : MonoBehaviour
{
    public void UnlockSkill(string name)
    {
        if (Game.Instance.UnlockSkill(name))
        {
            Flowchart.BroadcastFungusMessage(name);
        }
    }

    public void UnlockSkillAllSkill()
    {
        Game.Instance.UnlockAllSkill();
    }

}
