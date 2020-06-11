﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using DG.Tweening;

public class Pump : SkillInstanceBase
{
    private float m_StartTime = 0f;

    /// <summary>
    /// 技能选择器选中的单元
    /// </summary>
    private BaseUnit chooseUnit;
    /// <summary>
    /// 存放路径上单元的列表
    /// </summary>
    private List<BaseUnit> unitsOnPath;
    private GameObject skillEffect = null;

    public Pump(string name) : base(name)
    {
        m_SkillTrigers.Add(SkillTriggerType.Animation, new PlayAnimationTrigger("item_Pump", 0f, OnTriggerComplete));
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
                            Game.Instance.GetPlayerUnit().ExecuteSkill("item_Pump");
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
                            Game.Instance.GetPlayerUnit().ExecuteSkill("item_Pump");
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
                            Game.Instance.GetPlayerUnit().ExecuteSkill("item_Pump");
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
                            Game.Instance.GetPlayerUnit().ExecuteSkill("item_Pump");
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
        tweeners.Add(player.transform.DOLookAt(chooseUnit.Model.transform.position, 0.3f));

        WaitForSeconds waitTime = new WaitForSeconds(1f);
        yield return waitTime;
        //开始改变路径上单元的状态
        WaitForSeconds interval = new WaitForSeconds(0.2f);
        WeaponController.Instance.firePos.forward = Game.Instance.GetPlayerUnit().transform.forward;
        skillEffect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/Laser Toon Water", WeaponController.Instance.firePos.position);
        skillEffect.transform.forward = chooseUnit.transform.position - Game.Instance.GetPlayerUnit().transform.position;
        skillEffect.transform.GetChild(0).DOMove(unitsOnPath[unitsOnPath.Count - 1].transform.position, unitsOnPath.Count * 0.2f).SetEase(Ease.Linear);
        do
        {
            unitsOnPath[0].SetState(new Water(unitsOnPath[0]));
            unitsOnPath.RemoveAt(0);
            
            if(unitsOnPath.Count == 0)
            {
                yield return interval;
                GameObject.Destroy(skillEffect);
                break;
            }
            yield return interval;
        } while (unitsOnPath.Count != 0);
        WeaponController.Instance.EndPump();
        yield return new WaitForSeconds(1f);
        OnTriggerComplete();
        CoroutineManager.StopCoroutine(m_skillProcessCoroutine);
    }

    protected override void OnSkillEnd()
    {
        WeaponController.Instance.ResetImediately();
        if (skillEffect != null)
            GameObject.Destroy(skillEffect);
        base.OnSkillEnd();
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
            return unitsOnPath;
        }
        if (direction.z > 0.0001)
        {
            while (chooseUnit.Up != null && chooseUnit.Up.UpperGameObject == null)
            {
                unitsOnPath.Add(chooseUnit.Up);
                chooseUnit = chooseUnit.Up;
            }
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
            return unitsOnPath;
        }

        Debug.LogError("未正确比较出技能Pump选择的方向");
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