using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理SkillTrigger和Skill的触发
/// </summary>
public class SkillInstanceBase
{
    public string SkillName;
    public bool m_IsUsed = false;
    public SkillState m_SkillState = SkillState.End;
    public Dictionary<SkillTriggerType, ISkillTrigger> m_SkillTrigers = new Dictionary<SkillTriggerType, ISkillTrigger>();
    private int completeCount = 0;

    public SkillInstanceBase(string name) { SkillName = name; }

    public SkillInstanceBase(SkillInstanceBase other)
    {
        foreach(var trigger in other.m_SkillTrigers)
        {
            m_SkillTrigers.Add(trigger.Key, trigger.Value.Clone());
        }
    }

    public virtual void Init()
    {
        foreach (var trigger in m_SkillTrigers)
        {
            trigger.Value.Init();
        }
    }

    public virtual void Execute(ISkillCore instance)
    {
        m_SkillState = SkillState.Playing;
        foreach (var trigger in m_SkillTrigers)
        {
            trigger.Value.Execute(instance);
        }
    }

    public virtual void Reset()
    {
        foreach(var trigger in m_SkillTrigers)
        {
            trigger.Value.Reset();
        }
    }

    public int GetTriggerClone(string typeName)
    {
        int count = 0;
        foreach (var trigger in m_SkillTrigers)
        {
            if (trigger.Value.GetTypeName() == typeName)
                ++count;
        }
        return count;
    }

    public void OnTriggerComplete()
    {
        completeCount++;
        if (completeCount == m_SkillTrigers.Count + 1)
        {
            m_SkillState = SkillState.End;
            completeCount = 0;
            Game.Instance.SetCanInput(true);
            Debug.Log("技能结束");
        }
          
    }
}
