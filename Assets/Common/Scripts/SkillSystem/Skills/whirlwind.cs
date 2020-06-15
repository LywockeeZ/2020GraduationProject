using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

public class Whirlwind : SkillInstanceBase
{
    private float m_StartTime = 0.7f;
    private GameObject skillEffect;

    public Whirlwind(string name) : base(name)
    {
        m_SkillTrigers.Add(SkillTriggerType.Animation, new PlayAnimationTrigger("skill_Whirlwind", 0f, OnTriggerComplete));
    }
    public override void ShowIndicator()
    {
        Debug.Log("ShowIndicator:" + SkillName);
        Highlight(1, Game.Instance.GetPlayerUnit().CurrentOn);
    }

    public override void CloseIndicator()
    {
        Debug.Log("CloseIndicator:" + SkillName);
        Highlight(1, Game.Instance.GetPlayerUnit().CurrentOn, false);
    }

    public override void ShowEmitter()
    {
        base.ShowEmitter();
        Debug.Log("ShowEmittor:" + SkillName);
        Game.Instance.SetCanInput(false);
        Highlight(2, Game.Instance.GetPlayerUnit().CurrentOn);
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
        Highlight(2, Game.Instance.GetPlayerUnit().CurrentOn, false);
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
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("CanEmmit")))
            //{
            //    SkillIndicator indicator = hitInfo.transform.parent.gameObject.GetComponent<SkillIndicator>();
            //    indicator.SetIsMouseIn(true);

            //}
            //if (Input.GetMouseButtonDown(0))
            //{
                //indicator.HighlightCancel();
                Debug.Log("技能施放");
                Game.Instance.GetPlayerUnit().ExecuteSkill();
                isEnd = true;
                break;
            //}

            yield return null;
        } while (!isEnd);
        CoroutineManager.StopCoroutine(m_skillEmitterCoroutine);
    }


    protected override IEnumerator SkillProcess(ISkillCore instance)
    {
        Player player = (Player)instance.UpperUnit;
        BaseUnit baseUnit = player.CurrentOn;

        yield return new WaitForSeconds(0.1f);
        skillEffect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/skills/whirlwindEffect", baseUnit.transform.position + 0.3f * Vector3.up);
        skillEffect.transform.SetParent(player.transform);
        skillEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
        WaitForSeconds startTime =  new WaitForSeconds(m_StartTime);
        yield return startTime;

        NormalStageData stageData = baseUnit.GetStage();
        stageData.GetBaseUnit(baseUnit.x - 1, baseUnit.y);
        EndFire(stageData.GetBaseUnit(baseUnit.x - 1, baseUnit.y    ));
        EndFire(stageData.GetBaseUnit(baseUnit.x - 1, baseUnit.y + 1));
        EndFire(stageData.GetBaseUnit(baseUnit.x - 1, baseUnit.y - 1));
        EndFire(stageData.GetBaseUnit(baseUnit.x + 1, baseUnit.y    ));
        EndFire(stageData.GetBaseUnit(baseUnit.x + 1, baseUnit.y + 1));
        EndFire(stageData.GetBaseUnit(baseUnit.x + 1, baseUnit.y - 1));
        EndFire(stageData.GetBaseUnit(baseUnit.x    , baseUnit.y + 1));
        EndFire(stageData.GetBaseUnit(baseUnit.x    , baseUnit.y - 1));

        GameObject.Destroy(skillEffect);
        OnTriggerComplete();
        CoroutineManager.StopCoroutine(m_skillProcessCoroutine);
    }


    private void EndFire(BaseUnit unit)
    {
        if (unit != null && unit.UpperGameObject != null && unit.UpperUnit.ControlType == ENUM_UpperUnitControlType.Fixed)
        {
            unit.UpperGameObject.GetComponent<IFixedUnit>().Handle();
        }
        else
        if (unit != null && unit.State.StateType != ENUM_State.Block && unit.State.StateType != ENUM_State.Water)
        {
            unit.SetState(new Water(unit), null);
        }
    }


    private void HighlightTarget(BaseUnit unit, int type, bool isOn = true)
    {
        if (unit != null)
        {
            if (type == 1)
            {
                if (isOn)
                {
                    unit.State.Model.GetComponent<SkillIndicator>().ShowIndicator();
                }
                else
                    unit.State.Model.GetComponent<SkillIndicator>().HideIndicator();
            }
            else
            {
                if (isOn)
                {
                    unit.State.Model.GetComponent<SkillIndicator>().ShowEmitter();
                }
                else
                    unit.State.Model.GetComponent<SkillIndicator>().HideEmitter();

            }
        }
    }


    private void Highlight(int type, BaseUnit baseUnit, bool isOn = true)
    {
        NormalStageData stageData = baseUnit.GetStage();

        HighlightTarget(stageData.GetBaseUnit(baseUnit.x - 1, baseUnit.y    ), type, isOn);
        HighlightTarget(stageData.GetBaseUnit(baseUnit.x - 1, baseUnit.y + 1), type, isOn);
        HighlightTarget(stageData.GetBaseUnit(baseUnit.x - 1, baseUnit.y - 1), type, isOn);
        HighlightTarget(stageData.GetBaseUnit(baseUnit.x + 1, baseUnit.y    ), type, isOn);
        HighlightTarget(stageData.GetBaseUnit(baseUnit.x + 1, baseUnit.y + 1), type, isOn);
        HighlightTarget(stageData.GetBaseUnit(baseUnit.x + 1, baseUnit.y - 1), type, isOn);
        HighlightTarget(stageData.GetBaseUnit(baseUnit.x    , baseUnit.y + 1), type, isOn);
        HighlightTarget(stageData.GetBaseUnit(baseUnit.x    , baseUnit.y - 1), type, isOn);
    }

}
