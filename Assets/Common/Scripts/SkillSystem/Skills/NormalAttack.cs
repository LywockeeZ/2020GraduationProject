﻿using System.Collections;
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

    public override void Execute(ISkillCore instance)
    {
        base.Execute(instance);
        DoSkillLogic(instance);
    }

    private void DoSkillLogic(ISkillCore instance)
    {
        Game.Instance.SetCanInput(false);
        Player player = (Player)instance.UpperUnit;
        player.transform.DOLookAt(instance.TargetUnit.Model.transform.position, 0.5f);
        targetUnit = Game.Instance.GetPlayerUnit().TargetUnit;
        void action() { targetUnit.SetState(new Block(targetUnit)); };
        CoroutineManager.StartCoroutineTask(action, 0.5f);

        OnTriggerComplete();
    }

    protected override void OnSkillEnd()
    {
        base.OnSkillEnd();
        Game.Instance.GetPlayerUnit().MoveByNavMesh(targetUnit.Model.transform.position);
    }

}