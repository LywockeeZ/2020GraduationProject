﻿using System.Collections;
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

    public override void ShowEmitter()
    {
        Debug.Log("ShowEmittor:" + SkillName);
        Game.Instance.SetCanInput(false);
        Highlight(Color.red, Game.Instance.GetPlayerUnit().CurrentOn);

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
                    }
                }
                if (Game.Instance.GetPlayerUnit().CurrentOn.Down != null && Game.Instance.GetPlayerUnit().CurrentOn.Down.UpperGameObject == null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Down.Model.transform.position)
                    {
                        chooseUnit = Game.Instance.GetPlayerUnit().CurrentOn.Down;
                        Debug.Log("技能施放");
                        Game.Instance.GetPlayerUnit().ExecuteSkill();
                    }
                }
                if (Game.Instance.GetPlayerUnit().CurrentOn.Left != null && Game.Instance.GetPlayerUnit().CurrentOn.Left.UpperGameObject == null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Left.Model.transform.position)
                    {
                        chooseUnit = Game.Instance.GetPlayerUnit().CurrentOn.Left;
                        Debug.Log("技能施放");
                        Game.Instance.GetPlayerUnit().ExecuteSkill();
                    }
                }
                if (Game.Instance.GetPlayerUnit().CurrentOn.Right != null && Game.Instance.GetPlayerUnit().CurrentOn.Right.UpperGameObject == null)
                {
                    if (clickedPos == Game.Instance.GetPlayerUnit().CurrentOn.Right.Model.transform.position)
                    {
                        chooseUnit = Game.Instance.GetPlayerUnit().CurrentOn.Right;
                        Debug.Log("技能施放");
                        Game.Instance.GetPlayerUnit().ExecuteSkill();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Game.Instance.SetCanInput(true);
        }

    }

    public override void Execute(ISkillCore instance)
    {
        base.Execute(instance);
        void action() { DoSkillLogic(instance); }
        CoroutineManager.StartCoroutineTask(action, m_StartTime);
    }


    private void DoSkillLogic(ISkillCore instance)
    {
        Game.Instance.SetCanInput(false);
        Player player = (Player)instance.UpperUnit;

        //获取路径上所有单元
        unitsOnPath = FindUnitsOnPath(chooseUnit);
        player.transform.DOLookAt(chooseUnit.Model.transform.position, 0.3f).OnComplete(()=> {
            //开始移动
            player.Agent.acceleration = 10f;
            player.Agent.speed = 8f;
            player.MoveByNavMeshInSkill(finalUnitOnPath.Model.transform.position);
        });

        //开始改变路径上单元的状态
        CoroutineManager.StartCoroutine(EndFireProcess());

        bool JudgeIsEnd()
        {
            if (unitsOnPath.Count != 0)
            {
                return false;
            }
            else
                return true;
        }
        void OnEndFireProcessEnd()
        {
            player.Agent.velocity = Vector3.zero;
            player.Agent.acceleration = 6f;
            player.Agent.speed = 1.5f;
            player.UpdateUnit(finalUnitOnPath);
            player.SkillAnimator.SetTrigger("skill_SlashEnd");
        }
        CoroutineManager.StartCoroutineTask(JudgeIsEnd, OnEndFireProcessEnd, 0f);

        OnTriggerComplete();
    }

    IEnumerator EndFireProcess()
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.1f);
        do
        {
            Vector2 playerPos = new Vector2(Game.Instance.GetPlayerUnit().transform.position.x, Game.Instance.GetPlayerUnit().transform.position.z);
            Vector2 unitPos = new Vector2(unitsOnPath[0].Model.transform.position.x, unitsOnPath[0].Model.transform.position.z);
            if ((playerPos - unitPos).magnitude < 0.2f)
            {
                unitsOnPath[0].SetState(new Block(unitsOnPath[0]));
                unitsOnPath.RemoveAt(0);
            }
            yield return null;
        } while (unitsOnPath.Count != 0);
        CoroutineManager.StopCoroutine(EndFireProcess());
    }

    private List<BaseUnit> FindUnitsOnPath(BaseUnit chooseUnit)
    {
        List<BaseUnit> unitsOnPath = new List<BaseUnit>();
        Vector3 direction = (chooseUnit.Model.transform.position - Game.Instance.GetPlayerUnit().CurrentOn.Model.transform.position).normalized;
        unitsOnPath.Add(chooseUnit);
        if (direction.x > 0.0001)
        {
            while(chooseUnit.Right != null && chooseUnit.Right.UpperGameObject == null)
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
            while (chooseUnit.Left != null && chooseUnit.Left.UpperGameObject == null)
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
            while (chooseUnit.Up != null && chooseUnit.Up.UpperGameObject == null)
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
            while (chooseUnit.Down != null && chooseUnit.Down.UpperGameObject == null)
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

    private void EndFire(BaseUnit unit)
    {
        if (unit != null && unit.State.StateType != ENUM_State.Block)
        {
            unit.SetState(new Water(unit), null);
        }
    }


    private void HighlightTarget(BaseUnit unit, Color color)
    {
        if (unit != null && unit.UpperGameObject == null)
        {
            unit.Model.transform.GetChild(0).GetComponent<Highlighter>().Hover(color);
        }
    }

    private void Highlight(Color color, BaseUnit baseUnit)
    {
        Color hightlightColor = color;
        HighlightTarget(baseUnit?.Up, color);
        HighlightTarget(baseUnit?.Right, color);
        HighlightTarget(baseUnit?.Down, color);
        HighlightTarget(baseUnit?.Left, color);
    }

}
