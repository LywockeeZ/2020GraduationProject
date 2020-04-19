﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : IGameSystem
{
    /// <summary>
    /// 存放解锁技能的队列
    /// </summary>
    private Queue<SkillInstanceBase> m_skillToUnlock = new Queue<SkillInstanceBase>();

    /// <summary>
    /// 存放当前已经解锁的技能
    /// </summary>
    private Dictionary<string, SkillInstanceBase> m_UnlockSkills = new Dictionary<string, SkillInstanceBase>();

    private SkillInstanceBase MainSkill = null;

    private List<SkillInstanceBase> MainItems = new List<SkillInstanceBase>();

    public SkillSystem()
    {
        Initialize();
    }

    public override void Initialize()
    {
        RegisterEvent();
        UnlockSkill("NormalAttack");
    }

    public override void Update()
    {
        //Debug.Log(MainSkill);
    }

    public override void Release()
    {
        DetachEvent();
    }

    private EventListenerDelegate OnStageEnd;
    private EventListenerDelegate OnStageRestart;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageEnd,
            OnStageEnd = (Message evt) =>
            {
                SetMainSkill(null);
            });

        Game.Instance.RegisterEvent(ENUM_GameEvent.StageRestart,
            OnStageRestart = (Message evt) =>
            {
                SetMainSkill(null);
            });

    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.StageEnd, OnStageEnd);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageRestart, OnStageRestart);
    }

    public SkillInstanceBase GetSkill(string skillName)
    {
        SkillInstanceBase skillInstance = null;
        m_UnlockSkills.TryGetValue(skillName, out skillInstance);

        if (skillInstance == null)
        {
            Debug.LogError("技能[" + skillName + "]未解锁");
        }

        return skillInstance;
    }

    public void UnlockSkill(string skillName)
    {
        SkillInstanceBase skillInstance = GameFactory.GetSkillFactory().GetSkillInstance(skillName);
        m_UnlockSkills.Add(skillName, skillInstance);
    }

    public void UnlockAllSkill()
    {
        UnlockSkill("Whirlwind");
    }

    public void SetMainSkill(string skillName)
    {
        if (skillName == null)
        {
            MainSkill = null;
        }
        else
        {
            Debug.Log("Set Main Skill " + skillName);
            MainSkill = m_UnlockSkills[skillName];
        }
    }

    public void SetMainItems(List<string> itemsName)
    {

    }

    public List<SkillInstanceBase> GetMainItems()
    {
        return MainItems;
    }


    public SkillInstanceBase GetMainSkill()
    {
        return MainSkill;
    }

    public Dictionary<string, SkillInstanceBase> GetUnlockedSkills()
    {
        return m_UnlockSkills;
    }

    public Queue<SkillInstanceBase>GetSkillsToUnlock()
    {
        return m_skillToUnlock;
    }
}
