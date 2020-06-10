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
    public override void ShowIndicator()
    {
        Debug.Log("ShowIndicator:" + SkillName);
        Highlight(Color.blue, Game.Instance.GetPlayerUnit().CurrentOn);
    }

    public override void CloseIndicator()
    {
        Debug.Log("CloseIndicator:" + SkillName);
        Highlight(Color.blue, Game.Instance.GetPlayerUnit().CurrentOn, false);
    }

    public override void ShowEmitter()
    {
        base.ShowEmitter();
        Debug.Log("ShowEmittor:" + SkillName);
        Game.Instance.SetCanInput(false);
        Highlight(Color.red, Game.Instance.GetPlayerUnit().CurrentOn);
        m_skillEmitterCoroutine = CoroutineManager.StartCoroutineReturn(SkillEmitter());
    }

    /// <summary>
    /// 由外部调用，关闭技能发射器
    /// </summary>
    public override void CloseEmitter()
    {
        Debug.Log("CloseEmittor:" + SkillName);
        Game.Instance.SetCanInput(true);
        HideEmitter();
        CoroutineManager.StopCoroutine(m_skillEmitterCoroutine);
    }

    /// <summary>
    /// 由内部调用，仅仅隐藏技能发射器
    /// </summary>
    protected override void HideEmitter()
    {
        base.HideEmitter();
        Highlight(Color.red, Game.Instance.GetPlayerUnit().CurrentOn, false);
    }

    /// <summary>
    /// 用来执行技能释放器的逻辑
    /// </summary>
    /// <returns></returns>
    IEnumerator SkillEmitter()
    {
        bool isEnd = false;
        yield return null;
        do
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("技能施放");
                Game.Instance.GetPlayerUnit().ExecuteSkill();
                isEnd = true;
            }
            yield return null;
        } while (!isEnd);
        CoroutineManager.StopCoroutine(m_skillEmitterCoroutine);
    }


    protected override IEnumerator SkillProcess(ISkillCore instance)
    {
        WaitForSeconds startTime =  new WaitForSeconds(m_StartTime);
        yield return startTime;
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
        CoroutineManager.StopCoroutine(m_skillProcessCoroutine);
    }


    private void EndFire(BaseUnit unit)
    {
        if (unit != null && unit.State.StateType != ENUM_State.Block && unit.State.StateType != ENUM_State.Water)
        {
            unit.SetState(new Water(unit), null);
        }
    }


    private void HighlightTarget(BaseUnit unit, Color color, bool isOn = true)
    {
        if (unit != null)
        {
            if (isOn)
            {
                unit.Model.transform.GetChild(0).GetComponent<Highlighter>().ConstantOn(color, 0.1f);
            }
            else
                unit.Model.transform.GetChild(0).GetComponent<Highlighter>().ConstantOff(0.1f);
        }
    }


    private void Highlight(Color color, BaseUnit baseUnit, bool isOn = true)
    {
        HighlightTarget(baseUnit?.Up, color, isOn);
        HighlightTarget(baseUnit?.Up?.Right, color, isOn);
        HighlightTarget(baseUnit?.Right, color, isOn);
        HighlightTarget(baseUnit?.Right?.Down, color, isOn);
        HighlightTarget(baseUnit?.Down, color, isOn);
        HighlightTarget(baseUnit?.Down?.Left, color, isOn);
        HighlightTarget(baseUnit?.Left, color, isOn);
        HighlightTarget(baseUnit?.Left?.Up, color, isOn);
    }

}
