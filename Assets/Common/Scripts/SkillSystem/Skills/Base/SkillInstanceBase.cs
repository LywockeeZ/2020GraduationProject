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
    public SkillState m_SkillState = SkillState.Ready;
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

    /// <summary>
    /// 显示技能指示器
    /// </summary>
    public virtual void ShowIndicator()
    {

    }

    /// <summary>
    /// 关闭技能指示器
    /// </summary>
    public virtual void CloseIndicator()
    {

    }

    /// <summary>
    /// 显示技能发射器
    /// </summary>
    public virtual void ShowEmitter()
    {

    }

    /// <summary>
    /// 关闭技能发射器
    /// </summary>
    public virtual void CloseEmitter()
    {

    }

    /// <summary>
    /// 每一个触发器执行完时调用此函数，用来判断所有触发器是否执行完
    /// </summary>
    protected virtual void OnTriggerComplete()
    {
        completeCount++;
        if (completeCount == m_SkillTrigers.Count + 1)
        {
            m_SkillState = SkillState.End;
            completeCount = 0;
            OnSkillEnd();
            Debug.Log("技能结束");
        }
          
    }

    /// <summary>
    /// 每个技能一开始必须调用
    /// </summary>
    protected virtual void OnSkillStart()
    {
        m_SkillState = SkillState.Playing;
        Game.Instance.SetCanInput(false);
        Game.Instance.SetExecutingSkill(this);
    }

    protected virtual void OnSkillEnd()
    {
        m_SkillState = SkillState.Ready;
        Game.Instance.SetCanInput(true);
        Game.Instance.NotifyEvent(ENUM_GameEvent.SkillEnd);
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


}
