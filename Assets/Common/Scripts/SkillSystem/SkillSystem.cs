using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : IGameSystem
{
    private SkillInstanceBase skillExecuting = null;
    private SkillInstanceBase selectedSkill = null;

    /// <summary>
    /// 待执行的技能队列
    /// </summary>
    private Queue<SkillInstanceBase> m_skillToExecuting = new Queue<SkillInstanceBase>();

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
        UnlockSkill("skill_NormalAttack");
    }

    public override void Update()
    {
        //Debug.Log(MainSkill);
        //检测技能是否执行完
        if (skillExecuting != null && skillExecuting.m_SkillState == SkillState.Ready)
        {
            skillExecuting = null;
        }
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
                if(skillExecuting != null)
                {
                    skillExecuting.Reset();
                }
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

    public bool UnlockSkill(string skillName)
    {
        if (string.IsNullOrEmpty(skillName))
        {
            Debug.LogError("SkillName不能为空");
            return false;
        }
        else
        {
            SkillInstanceBase skill = null;
            m_UnlockSkills.TryGetValue(skillName, out skill);
            if (skill == null)
            {
                SkillInstanceBase skillInstance = GameFactory.GetSkillFactory().GetSkillInstance(skillName);
                if (skillName != "skill_NormalAttack")
                {
                    m_skillToUnlock.Enqueue(skillInstance);
                }
                m_UnlockSkills.Add(skillName, skillInstance);
                return true;
            }
            else return false;
        }
    }

    public void UnlockAllSkill()
    {
        UnlockSkill("skill_Whirlwind");
        UnlockSkill("skill_Slash");
        UnlockSkill("item_WaterSac");
        UnlockSkill("item_Pump");
        UnlockSkill("item_WaterBag");
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
        for (int i = 0; i < itemsName.Count; i++)
        {
            if (itemsName[i] != null)
            {
                MainItems.Add(m_UnlockSkills[itemsName[i]]);
            }
            else MainItems.Add(null);
        }
    }

    public List<SkillInstanceBase> GetMainItems()
    {
        return MainItems;
    }


    public SkillInstanceBase GetMainSkill()
    {
        return MainSkill;
    }

    public SkillInstanceBase GetExecutingSkill()
    {
        return skillExecuting;
    }

    public void SetExecutingSkill(SkillInstanceBase skill)
    {
        skillExecuting = skill;
    }

    public void SetSelectedSkill(SkillInstanceBase skill)
    {
        selectedSkill = skill;
    }

    public SkillInstanceBase GetSelectedSkill()
    {
        return selectedSkill;
    }

    public Dictionary<string, SkillInstanceBase> GetUnlockedSkills()
    {
        return m_UnlockSkills;
    }

    public Queue<SkillInstanceBase>GetSkillsToUnlock()
    {
        return m_skillToUnlock;
    }

    public void ClearUnlockedSkill()
    {
        foreach (KeyValuePair<string, SkillInstanceBase> skill in m_UnlockSkills)
        {
            if(skill.Key != "skill_NormalAttack")
                m_UnlockSkills.Remove(skill.Key);
        }
        
    }
}
