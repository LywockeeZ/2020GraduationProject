using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayAnimationTrigger : AbstractSkillTrigger
{
    private string m_AnimationName;
    private Action m_callBack = null;

    private PlayAnimationTrigger() { }

    public PlayAnimationTrigger(string name, float startTime, Action onTriggerComplete)
    {
        OnTriggerComplete = onTriggerComplete;
        m_StartTime = startTime;
        m_AnimationName = name;
    }

    public PlayAnimationTrigger(string name, float startTime, Action onTriggerComplete, Action callBack)
    {
        OnTriggerComplete = onTriggerComplete;
        m_StartTime = startTime;
        m_AnimationName = name;
        m_callBack = callBack;
    }


    public override void Init()
    {

    }

    public override bool Execute(ISkillCore instance)
    {
        //InitEvent(instance.SkillAnimator);
        instance.SkillAnimator.SetTrigger(m_AnimationName, delegate() { OnAnimationEnd(); });
        return true;
    }

    public override ISkillTrigger Clone()
    {
        throw new System.NotImplementedException();
    }

    public void InitEvent(Animator animator)
    {
        AnimationClip[] infos = animator.runtimeAnimatorController.animationClips;
        int index = -1;
        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i].name == m_AnimationName)
            {
                index = i;
            }
        }

        if (index == -1)
        {
            Debug.LogError("未能找到[" + m_AnimationName + "]动画片段");
        }

        AnimationEvent animationEndEvent = new AnimationEvent();
        animationEndEvent.functionName = "OnAnimationEnd";
        animationEndEvent.time = infos[index].length;
        infos[index].AddEvent(animationEndEvent);
    }

    public void OnAnimationEnd()
    {
        OnTriggerComplete();
        if (m_callBack != null)
        {
            m_callBack();
        }
    }


}
