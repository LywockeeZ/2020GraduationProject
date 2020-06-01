using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using DG.Tweening;
using System;
using MoreMountains.Tools;
using Fungus;

public class WaterSac : SkillInstanceBase
{
    private Coroutine m_coroutine;
    private float m_StartTime = 0f;
    private List<BaseUnit> targetUnits = new List<BaseUnit>();
    private BaseUnit chooseUnit;
    private GameObject itemTrail;
    private GameObject shuinang;
    private MMBezierLineRenderer bezierLine = null;
    private Vector3[] path;

    public WaterSac(string name) : base(name)
    {
        m_SkillTrigers.Add(SkillTriggerType.Animation, new PlayAnimationTrigger("item_WaterSac", 0f, OnTriggerComplete));
    }

    public override void ShowIndicator()
    {
        Debug.Log("ShowIndicator:" + SkillName);
        FindTargetUnitsAndHighlight();
    }

    public override void CloseIndicator()
    {
        Debug.Log("CloseIndicator:" + SkillName);
        Highlight(Color.blue, false);
        targetUnits.Clear();
    }

    public override void ShowEmitter()
    {
        Debug.Log("ShowEmittor:" + SkillName);
        Highlight(Color.red, true);
        Game.Instance.SetCanInput(false);
        m_coroutine = CoroutineManager.StartCoroutineReturn(SkillEmitter());
    }

    /// <summary>
    /// 由外部调用，关闭技能发射器
    /// </summary>
    public override void CloseEmitter()
    {
        Debug.Log("CloseEmittor:" + SkillName);
        Game.Instance.SetCanInput(true);
        HideEmitter();
        CoroutineManager.StopCoroutine(m_coroutine);
    }

    /// <summary>
    /// 由内部调用，仅仅隐藏技能发射器
    /// </summary>
    private void HideEmitter()
    {
        Highlight(Color.blue, false);
        itemTrail?.SetActive(false);
        targetUnits.Clear();
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
                GameObject hitObj = hitInfo.transform.gameObject;
                Highlighter highlighter = hitObj.GetComponent<Highlighter>();
                if (highlighter != null)
                {
                    //如果是蓝色，说明该目标为可选单元
                    if (highlighter.color == Color.red)
                    {
                        //highlighter.Hover(Color.red);
                        DrawCruve(Game.Instance.GetPlayerUnit().transform.position + 0.2f * Game.Instance.GetPlayerUnit().transform.forward, hitObj.transform.position);

                        if (Input.GetMouseButtonDown(0))
                        {
                            chooseUnit = Game.Instance.GetCurrentStage().GetBaseUnit(Convert.ToInt16(hitInfo.transform.parent.localPosition.x), Convert.ToInt16(hitInfo.transform.parent.localPosition.z));
                            Debug.Log("技能施放");
                            Game.Instance.GetPlayerUnit().ExecuteSkill("item_WaterSac");
                            isEnd = true;
                        }

                    }
                    else itemTrail?.SetActive(false);
                }
            }
            else itemTrail?.SetActive(false);
            yield return null;
        } while (!isEnd);
        CoroutineManager.StopCoroutine(m_coroutine);
    }


    public override void Execute(ISkillCore instance)
    {
        OnSkillStart();
        HideEmitter();

        //先将技能逻辑注册给动画，再开始触发动画触发器
        WeaponController.Instance.setWaterSac += setWaterSac;
        WeaponController.Instance.throwWaterSac += throwWaterSac;
        WeaponController.Instance.EndKnife();

        Player player = (Player)instance.UpperUnit;
        player.transform.DOLookAt(chooseUnit.Model.transform.position, 0.5f).OnComplete(() => {
            base.Execute(instance);
        });

    }

    void setWaterSac()
    {
        shuinang = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Prefabs/Items/daoju_shuinang", WeaponController.Instance.handPos.position);
        shuinang.transform.SetParent(WeaponController.Instance.handPos);
    }

    void throwWaterSac()
    {
        DoSkillLogic();
    }



    private void DoSkillLogic()
    {
        shuinang.transform.SetParent(null);
        //shuinang.transform.GetChild(0).DOLocalRotate(new Vector3, 1.5f);
        shuinang.transform.DOLocalPath(bezierLine.path, 0.8f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
        {
            GameObject.Destroy(shuinang);
            if (chooseUnit.UpperUnit.ControlType == ENUM_UpperUnitControlType.Fixed)
            {
                chooseUnit.UpperGameObject.GetComponent<IFixedUnit>().Handle(false);
            }
            else
                chooseUnit.SetState(new Water(chooseUnit));
            OnTriggerComplete();
        });


    }

    protected override void OnSkillEnd()
    {
        base.OnSkillEnd();
        WeaponController.Instance.StartKnife();
        WeaponController.Instance.setWaterSac -= setWaterSac;
        WeaponController.Instance.throwWaterSac -= throwWaterSac;
    }

    /// <summary>
    /// 找到所有可选择的目标单元并高亮
    /// </summary>
    private void FindTargetUnitsAndHighlight()
    {
        Player player = Game.Instance.GetPlayerUnit();
        BaseUnit unitOnUp = player.CurrentOn.Up;
        BaseUnit unitOnDown = player.CurrentOn.Down;
        BaseUnit unitOnLeft = player.CurrentOn.Left;
        BaseUnit unitOnRight = player.CurrentOn.Right;
        int i = 0;

        while (unitOnUp != null || unitOnDown != null || unitOnLeft != null || unitOnRight != null)
        {
            if (unitOnUp != null && (unitOnUp.UpperUnit.ControlType != ENUM_UpperUnitControlType.Movable || unitOnUp.UpperUnit.ControlType != ENUM_UpperUnitControlType.NULL))
            {
                targetUnits.Add(unitOnUp);
                HighlightTarget(Color.blue, unitOnUp);
                unitOnUp = unitOnUp.Up;
            }
            if (unitOnDown != null && (unitOnDown.UpperUnit.ControlType != ENUM_UpperUnitControlType.Movable || unitOnDown.UpperUnit.ControlType != ENUM_UpperUnitControlType.NULL))
            {
                targetUnits.Add(unitOnDown);
                HighlightTarget(Color.blue, unitOnDown);
                unitOnDown = unitOnDown.Down;
            }
            if (unitOnLeft != null && (unitOnLeft.UpperUnit.ControlType != ENUM_UpperUnitControlType.Movable || unitOnLeft.UpperUnit.ControlType != ENUM_UpperUnitControlType.NULL))
            {
                targetUnits.Add(unitOnLeft);
                HighlightTarget(Color.blue, unitOnLeft);
                unitOnLeft = unitOnLeft.Left;
            }
            if (unitOnRight != null && (unitOnRight.UpperUnit.ControlType != ENUM_UpperUnitControlType.Movable || unitOnRight.UpperUnit.ControlType != ENUM_UpperUnitControlType.NULL))
            {
                targetUnits.Add(unitOnRight);
                HighlightTarget(Color.blue, unitOnRight);
                unitOnRight = unitOnRight.Right;
            }
            i++;
            if (i == 4)
                break;
        }

    }

    private void DrawCruve(Vector3 startPos, Vector3 endPos)
    {
        if (bezierLine == null)
        {
            itemTrail = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Prefabs/Others/ItemTrail", Vector3.zero);
            GameObject.DontDestroyOnLoad(itemTrail);
            bezierLine = itemTrail.transform.GetChild(0).GetComponent<MMBezierLineRenderer>();
        }
        else itemTrail.SetActive(true);
        Vector3 upOffset = new Vector3(0, 1, 0);
        itemTrail.transform.GetChild(0).transform.position = startPos;
        itemTrail.transform.GetChild(3).transform.position = endPos;
        itemTrail.transform.GetChild(1).transform.position = (endPos - startPos) / 3 + startPos + upOffset * (endPos - startPos).magnitude * 2/3;
        itemTrail.transform.GetChild(2).transform.position = (endPos - startPos) / 3 * 2 + startPos + upOffset * (endPos - startPos).magnitude * 2/3;

        bezierLine.DrawCurve();
    }

    private void EndFire(BaseUnit unit)
    {
        if (unit != null && unit.State.StateType != ENUM_State.Block)
        {
            unit.SetState(new Water(unit), null);
        }
    }


    private void HighlightTarget(Color color, BaseUnit unit, bool isOn = true)
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


    private void Highlight(Color color, bool isOn = true)
    {
        if (targetUnits != null)
        {
            foreach (var unit in targetUnits)
            {
                HighlightTarget(color, unit, isOn);
            }
        }
    }

}
