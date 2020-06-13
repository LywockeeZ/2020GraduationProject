using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using DG.Tweening;
using System;

public class Slash : SkillInstanceBase
{
    private float m_StartTime = 0f;
    /// <summary>
    /// 技能选择器选中的单元
    /// </summary>
    private BaseUnit chooseUnit;
    /// <summary>
    /// 路径上最后一个单元
    /// </summary>
    private BaseUnit finalUnitOnPath;
    /// <summary>
    /// 路径尽头相邻的单元
    /// </summary>
    private BaseUnit pathNextUnit;
    /// <summary>
    /// 存放路径上单元的列表
    /// </summary>
    private List<BaseUnit> unitsOnPath;     



    public Slash(string name) : base(name)
    {
        m_SkillTrigers.Add(SkillTriggerType.Animation, new PlayAnimationTrigger("skill_Slash", 0f, OnTriggerComplete));
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

    public override void CloseEmitter()
    {
        Debug.Log("CloseEmittor:" + SkillName);
        Game.Instance.SetCanInput(true);
        HideEmitter();
        CoroutineManager.StopCoroutine(m_skillEmitterCoroutine);
    }

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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("BaseUnit")))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 clickedPos = hitInfo.transform.position;
                    if (Game.Instance.GetPlayerUnit().CurrentOn.Up != null && Game.Instance.GetPlayerUnit().CurrentOn.Up.UpperGameObject == null)
                    {
                        if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Up.Model.transform.position)
                        {
                            chooseUnit = Game.Instance.GetPlayerUnit().CurrentOn.Up;
                            Debug.Log("技能施放");
                            Game.Instance.GetPlayerUnit().ExecuteSkill();
                            isEnd = true;
                            break;
                        }
                    }
                    if (Game.Instance.GetPlayerUnit().CurrentOn.Down != null && Game.Instance.GetPlayerUnit().CurrentOn.Down.UpperGameObject == null)
                    {
                        if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Down.Model.transform.position)
                        {
                            chooseUnit = Game.Instance.GetPlayerUnit().CurrentOn.Down;
                            Debug.Log("技能施放");
                            Game.Instance.GetPlayerUnit().ExecuteSkill();
                            isEnd = true;
                            break;
                        }
                    }
                    if (Game.Instance.GetPlayerUnit().CurrentOn.Left != null && Game.Instance.GetPlayerUnit().CurrentOn.Left.UpperGameObject == null)
                    {
                        if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Left.Model.transform.position)
                        {
                            chooseUnit = Game.Instance.GetPlayerUnit().CurrentOn.Left;
                            Debug.Log("技能施放");
                            Game.Instance.GetPlayerUnit().ExecuteSkill();
                            isEnd = true;
                            break;
                        }
                    }
                    if (Game.Instance.GetPlayerUnit().CurrentOn.Right != null && Game.Instance.GetPlayerUnit().CurrentOn.Right.UpperGameObject == null)
                    {
                        if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Right.Model.transform.position)
                        {
                            chooseUnit = Game.Instance.GetPlayerUnit().CurrentOn.Right;
                            Debug.Log("技能施放");
                            Game.Instance.GetPlayerUnit().ExecuteSkill();
                            isEnd = true;
                            break;
                        }
                    }
                }
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

        //获取路径上所有单元
        unitsOnPath = FindUnitsOnPath(chooseUnit);
        tweeners.Add(player.transform.DOLookAt(chooseUnit.Model.transform.position, 0.3f).OnComplete(()=> {
            //开始移动
            player.Agent.acceleration = 10f;
            player.Agent.speed = 8f;
            player.MoveByNavMeshInSkill(finalUnitOnPath.Model.transform.position);
        }));

        WaitForSeconds waitTime = new WaitForSeconds(0.1f);
        yield return waitTime;
        //开始改变路径上单元的状态
        do
        {
            Vector2 playerPos = new Vector2(Game.Instance.GetPlayerUnit().transform.position.x, Game.Instance.GetPlayerUnit().transform.position.z);
            Vector2 unitPos = new Vector2(unitsOnPath[0].transform.position.x, unitsOnPath[0].transform.position.z);
            if ((playerPos - unitPos).magnitude < 0.2f)
            {
                if (unitsOnPath.Count == 1)
                {
                    unitsOnPath[0].SetState(new Block(unitsOnPath[0]));
                }
                else
                {
                    if (unitsOnPath[0].State.StateType == ENUM_State.Fire)
                        unitsOnPath[0].SetState(new Ground(unitsOnPath[0]));
                }
                unitsOnPath.RemoveAt(0);
            }
            
            if(unitsOnPath.Count == 0)
            {
                OnEndFireProcessEnd();
            }
            yield return null;
        } while (unitsOnPath.Count != 0);
        CoroutineManager.StopCoroutine(m_skillProcessCoroutine);
    }


    private void OnEndFireProcessEnd()
    {
        Player player = Game.Instance.GetPlayerUnit();
        player.Agent.velocity = Vector3.zero;
        player.Agent.acceleration = 6f;
        player.Agent.speed = 1.5f;
        player.UpdateUnit(finalUnitOnPath);
        player.SkillAnimator.SetTrigger("skill_SlashEnd");
        if (pathNextUnit != null)
        {
            if (pathNextUnit.UpperUnit.ControlType == ENUM_UpperUnitControlType.Fixed)
            {
                pathNextUnit.UpperGameObject.GetComponent<IFixedUnit>().Handle(false);
            }
            pathNextUnit = null;
        }

         OnTriggerComplete();

    }

    protected override void OnSkillEnd()
    {
        base.OnSkillEnd();
        Player player = Game.Instance.GetPlayerUnit();
        player.Agent.velocity = Vector3.zero;
        player.Agent.acceleration = 6f;
        player.Agent.speed = 1.5f;
    }

    private List<BaseUnit> FindUnitsOnPath(BaseUnit chooseUnit)
    {
        List<BaseUnit> unitsOnPath = new List<BaseUnit>();
        Vector3 direction = (chooseUnit.Model.transform.position - Game.Instance.GetPlayerUnit().CurrentOn.Model.transform.position).normalized;
        unitsOnPath.Add(chooseUnit);
        if (direction.x > 0.0001)
        {
            while((chooseUnit.Right != null && chooseUnit.Right.UpperGameObject == null) || (chooseUnit.Right != null && chooseUnit.Right.UpperUnit.Type == ENUM_UpperUnit.Bee))
            {
                unitsOnPath.Add(chooseUnit.Right);
                chooseUnit = chooseUnit.Right;
            }
            if (chooseUnit.Right?.UpperGameObject != null)
            {
                pathNextUnit = chooseUnit.Right;
            }
            finalUnitOnPath = unitsOnPath[unitsOnPath.Count - 1];
            return unitsOnPath;
        }
        else
        if (direction.x < -0.0001)
        {
            while ((chooseUnit.Left != null && chooseUnit.Left.UpperGameObject == null) || (chooseUnit.Left != null && chooseUnit.Left.UpperUnit.Type == ENUM_UpperUnit.Bee))
            {
                unitsOnPath.Add(chooseUnit.Left);
                chooseUnit = chooseUnit.Left;
            }
            if (chooseUnit.Left?.UpperGameObject != null)
            {
                pathNextUnit = chooseUnit.Left;
            }
            finalUnitOnPath = unitsOnPath[unitsOnPath.Count - 1];
            return unitsOnPath;
        }

        if (direction.z > 0.0001)
        {
            while ((chooseUnit.Up != null && chooseUnit.Up.UpperGameObject == null) || (chooseUnit.Up != null && chooseUnit.Up.UpperUnit.Type == ENUM_UpperUnit.Bee))
            {
                unitsOnPath.Add(chooseUnit.Up);
                chooseUnit = chooseUnit.Up;
            }
            if (chooseUnit.Up?.UpperGameObject != null)
            {
                pathNextUnit = chooseUnit.Up;
            }
            finalUnitOnPath = unitsOnPath[unitsOnPath.Count - 1];
            return unitsOnPath;
        }
        else
        if (direction.z < -0.0001)
        {
            while ((chooseUnit.Down != null && chooseUnit.Down.UpperGameObject == null) || (chooseUnit.Down != null && chooseUnit.Down.UpperUnit.Type == ENUM_UpperUnit.Bee))
            {
                unitsOnPath.Add(chooseUnit.Down);
                chooseUnit = chooseUnit.Down;
            }
            if (chooseUnit.Down?.UpperGameObject != null)
            {
                pathNextUnit = chooseUnit.Down;
            }
            finalUnitOnPath = unitsOnPath[unitsOnPath.Count - 1];
            return unitsOnPath;
        }

        Debug.LogError("未正确比较出技能Slash选择的方向");
        return unitsOnPath;
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
        Color hightlightColor = color;
        HighlightTarget(baseUnit?.Up, color, isOn);
        HighlightTarget(baseUnit?.Right, color, isOn);
        HighlightTarget(baseUnit?.Down, color, isOn);
        HighlightTarget(baseUnit?.Left, color, isOn);
    }

}
