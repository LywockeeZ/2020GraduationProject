using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AbstractSkillTrigger :ISkillTrigger
{
    protected Action OnTriggerComplete; 

    protected float m_StartTime = 0;
    protected bool m_IsExecuted = false;
    protected string m_TypeName;

    public abstract void Init();
    public virtual void Reset() { m_IsExecuted = false; }
    public abstract ISkillTrigger Clone();
    public abstract bool Execute(ISkillCore instance);

    public float GetStartTime() { return m_StartTime; }
    public bool IsExecuted() { return m_IsExecuted; }
    public string GetTypeName() { return m_TypeName; }
}
