using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NormalAttack : SkillInstanceBase
{
    private float m_StartTime = 0f;
    private BaseUnit targetUnit;

    public NormalAttack(string name) : base(name)
    {
        m_SkillTrigers.Add(SkillTriggerType.Animation, new PlayAnimationTrigger("skill_NormalAttack", 0f, OnTriggerComplete));
    }


    protected override IEnumerator SkillProcess(ISkillCore instance)
    {
        WaitForSeconds startTime =  new WaitForSeconds(m_StartTime);
        yield return startTime;
        Player player = (Player)instance.UpperUnit;
        player.transform.DOLookAt(instance.TargetUnit.Model.transform.position, 0.5f);
        targetUnit = Game.Instance.GetPlayerUnit().TargetUnit;
        void action() { targetUnit.SetState(new Block(targetUnit), null); };
        CoroutineManager.StartCoroutineTask(action, 0.5f);
        OnTriggerComplete();
        CoroutineManager.StopCoroutine(m_skillProcessCoroutine);
    }
    
    protected override void OnSkillStart()
    {
        m_SkillState = SkillState.Playing;
        Game.Instance.SetCanInput(false);
    }

    protected override void OnSkillEnd()
    {
        m_SkillState = SkillState.Ready;
        if (!Game.Instance.IsCurrentStageEnd())
        {
            Game.Instance.SetCanInput(true);
        }
        Game.Instance.GetPlayerUnit().MoveByNavMesh(targetUnit.Model.transform.position);
    }

}
