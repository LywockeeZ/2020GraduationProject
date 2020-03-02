using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlwind : SkillInstanceBase
{
    private float m_StartTime = 0.8f;

    public Whirlwind(string name) : base(name)
    {
        m_SkillTrigers.Add(SkillTriggerType.Animation, new PlayAnimationTrigger("skill_Whirlwind", 0f, OnTriggerComplete));
    }

    public override void Execute(ISkillCore instance)
    {
        base.Execute(instance);      
        void action() { DoSkillLogic(instance);}
        CoroutineManager.StartCoroutineTask(action, m_StartTime);
    }

    public void DoSkillLogic(ISkillCore instance)
    {
        Game.Instance.SetCanInput(false);
        Player player = (Player)instance.UpperUnit;
        BaseUnit baseUnit = player.CurrentOn;

        EndFire(baseUnit?.Up);
        EndFire(baseUnit?.Up?.Right);
        EndFire(baseUnit?.Right);
        EndFire(baseUnit?.Right?.Down);
        EndFire(baseUnit?.Down);
        EndFire(baseUnit?.Down?.Left);
        EndFire(baseUnit?.Left);
        EndFire(baseUnit?.Left?.Up);
        OnTriggerComplete();
    }

    public void EndFire(BaseUnit unit)
    {
        if (unit != null && unit.State.StateType == ENUM_State.Fire )
        {
            unit.SetState(new Ground(unit));
        }
    }
}
