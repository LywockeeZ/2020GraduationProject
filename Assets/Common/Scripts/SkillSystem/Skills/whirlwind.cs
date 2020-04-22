using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

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

    private void DoSkillLogic(ISkillCore instance)
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

    private void EndFire(BaseUnit unit)
    {
        if (unit != null && unit.State.StateType == ENUM_State.Fire )
        {
            unit.SetState(new Ground(unit));
        }
    }

    public override void ShowIndicator()
    {
        Debug.Log("ShowIndicator:" + SkillName);
        Highlight(Color.blue, Game.Instance.GetPlayerUnit().CurrentOn);
    }

    public override void ShowEmitter()
    {
        Debug.Log("ShowEmittor:" + SkillName);
        Game.Instance.SetCanInput(false);
        Highlight(Color.red, Game.Instance.GetPlayerUnit().CurrentOn);
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("技能施放");
            Game.Instance.GetPlayerUnit().ExecuteSkill();
        }
    }

    private void HighlightTarget(BaseUnit unit, Color color)
    {
        if (unit != null)
        {
            unit.Model.transform.GetChild(0).GetComponent<Highlighter>().Hover(color);
        }
    }

    private void Highlight(Color color, BaseUnit baseUnit)
    {
        Color hightlightColor = color;
        HighlightTarget(baseUnit?.Up, color);
        HighlightTarget(baseUnit?.Up?.Right, color);
        HighlightTarget(baseUnit?.Right, color);
        HighlightTarget(baseUnit?.Right?.Down, color);
        HighlightTarget(baseUnit?.Down, color);
        HighlightTarget(baseUnit?.Down?.Left, color);
        HighlightTarget(baseUnit?.Left, color);
        HighlightTarget(baseUnit?.Left?.Up, color);
    }

}
