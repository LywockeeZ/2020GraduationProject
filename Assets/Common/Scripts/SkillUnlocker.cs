using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnlocker : MonoBehaviour
{
    public void UnlockSkill(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("SkillName不能为空");
        }
        else
            Game.Instance.UnlockSkill(name);
    }

}
