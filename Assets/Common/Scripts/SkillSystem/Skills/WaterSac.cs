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
    private float m_StartTime = 0f;
    private List<BaseUnit> targetUnits = new List<BaseUnit>();
    private BaseUnit chooseUnit;
    private GameObject itemTrail;
    private GameObject waterSac;
    private MMBezierLineRenderer bezierLine = null;
    private Vector3[] path;
    private bool isSkillStart = false;

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
        Highlight(1, false);
        targetUnits.Clear();
    }

    public override void ShowEmitter()
    {
        base.ShowEmitter();
        Debug.Log("ShowEmittor:" + SkillName);
        Highlight(2, true);
        Game.Instance.SetCanInput(false);
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
        Highlight(2, false);
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
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("CanEmmit")))
            {
                SkillIndicator indicator = hitInfo.transform.parent.gameObject.GetComponent<SkillIndicator>();
                indicator.SetIsMouseIn(true);
                DrawCruve(Game.Instance.GetPlayerUnit().transform.position + 0.2f * Game.Instance.GetPlayerUnit().transform.forward, hitInfo.transform.position);

                if (Input.GetMouseButtonDown(0))
                {
                    chooseUnit = hitInfo.transform.parent.parent.GetComponent<BaseUnit>();
                    indicator.HighlightCancel();
                    Debug.Log("技能施放");
                    Game.Instance.GetPlayerUnit().ExecuteSkill("item_WaterSac");
                    isEnd = true;
                    break;
                }
            }
            else itemTrail?.SetActive(false);


            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("BaseUnit")))
            //{
            //    GameObject hitObj = hitInfo.transform.gameObject;
            //    Highlighter highlighter = hitObj.GetComponent<Highlighter>();
            //    if (highlighter != null)
            //    {
            //        //如果是蓝色，说明该目标为可选单元
            //        if (highlighter.color == Color.red)
            //        {
            //            //highlighter.Hover(Color.red);
            //            DrawCruve(Game.Instance.GetPlayerUnit().transform.position + 0.2f * Game.Instance.GetPlayerUnit().transform.forward, hitObj.transform.position);

            //            if (Input.GetMouseButtonDown(0))
            //            {
            //                chooseUnit = Game.Instance.GetCurrentStage().GetBaseUnit(Convert.ToInt16(hitInfo.transform.parent.localPosition.x), Convert.ToInt16(hitInfo.transform.parent.localPosition.z));
            //                Debug.Log("技能施放");
            //                Game.Instance.GetPlayerUnit().ExecuteSkill("item_WaterSac");
            //                isEnd = true;
            //            }

            //        }
            //        else itemTrail?.SetActive(false);
            //    }
            //}
            //else itemTrail?.SetActive(false);
            yield return null;
        } while (!isEnd);
        CoroutineManager.StopCoroutine(m_skillEmitterCoroutine);
    }


    public override void Execute(ISkillCore instance)
    {
        //先将技能逻辑注册给动画，再开始触发动画触发器
        WeaponController.Instance.setWaterSac += setWaterSac;
        WeaponController.Instance.throwWaterSac += throwWaterSac;
        WeaponController.Instance.EndKnife();
        base.Execute(instance);
    }

    void setWaterSac()
    {
        waterSac = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Prefabs/Items/daoju_shuinang", WeaponController.Instance.rightHandPos.position);
        waterSac.transform.SetParent(WeaponController.Instance.rightHandPos);
    }

    void throwWaterSac()
    {
        isSkillStart = true;
    }

    protected override IEnumerator SkillProcess(ISkillCore instance)
    {
        Player player = (Player)instance.UpperUnit;
        player.Audio.clip = player.GetComponent<MyAudios>().audioClips[5];
        player.Audio.loop = false;
        player.Audio.volume = 0.7f;

        WaitForSeconds startTime =  new WaitForSeconds(m_StartTime);
        yield return startTime;

        tweeners.Add(player.transform.DOLookAt(chooseUnit.Model.transform.position, 0.5f));
        WaitForSeconds time = new WaitForSeconds(0.5f);
        yield return time;
        player.Audio.PlayDelayed(0.5f);

        while (!isSkillStart)
        {
            yield return null;
        }

        waterSac.transform.SetParent(null);
        //shuinang.transform.GetChild(0).DOLocalRotate(new Vector3, 1.5f);
        tweeners.Add(waterSac.transform.DOLocalPath(bezierLine.path, 0.8f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
        {
            GameObject.Destroy(waterSac);
            if (chooseUnit.UpperUnit.ControlType == ENUM_UpperUnitControlType.Fixed)
            {
                chooseUnit.UpperGameObject.GetComponent<IFixedUnit>().Handle(false);
            }
            else
                chooseUnit.SetState(new Water(chooseUnit));
            OnTriggerComplete();
            Game.Instance.NotifyEvent(ENUM_GameEvent.SetWaterTexture);
            CoroutineManager.StopCoroutine(m_skillProcessCoroutine);
        }));
    }


    protected override void OnSkillEnd()
    {
        WeaponController.Instance.StartKnife();
        WeaponController.Instance.setWaterSac -= setWaterSac;
        WeaponController.Instance.throwWaterSac -= throwWaterSac;
        isSkillStart = false;
        if(waterSac != null)
            GameObject.Destroy(waterSac);
        CoroutineManager.StopCoroutine(m_skillEmitterCoroutine);
        base.OnSkillEnd();
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
                HighlightTarget(unitOnUp , 1);
                unitOnUp = unitOnUp.Up;
            }
            if (unitOnDown != null && (unitOnDown.UpperUnit.ControlType != ENUM_UpperUnitControlType.Movable || unitOnDown.UpperUnit.ControlType != ENUM_UpperUnitControlType.NULL))
            {
                targetUnits.Add(unitOnDown);
                HighlightTarget(unitOnDown, 1);
                unitOnDown = unitOnDown.Down;
            }
            if (unitOnLeft != null && (unitOnLeft.UpperUnit.ControlType != ENUM_UpperUnitControlType.Movable || unitOnLeft.UpperUnit.ControlType != ENUM_UpperUnitControlType.NULL))
            {
                targetUnits.Add(unitOnLeft);
                HighlightTarget(unitOnLeft, 1);
                unitOnLeft = unitOnLeft.Left;
            }
            if (unitOnRight != null && (unitOnRight.UpperUnit.ControlType != ENUM_UpperUnitControlType.Movable || unitOnRight.UpperUnit.ControlType != ENUM_UpperUnitControlType.NULL))
            {
                targetUnits.Add(unitOnRight);
                HighlightTarget(unitOnRight, 1);
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
            unit.SetState(new Water(unit));
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
                    unit.State.Model.GetComponent<SkillIndicator>()?.ShowIndicator();
                }
                else
                    unit.State.Model.GetComponent<SkillIndicator>()?.HideIndicator();
            }
            else
            {
                if (isOn)
                {
                    unit.State.Model.GetComponent<SkillIndicator>()?.ShowEmitter();
                }
                else
                    unit.State.Model.GetComponent<SkillIndicator>()?.HideEmitter();
            }
        }
    }


    private void Highlight(int type, bool isOn = true)
    {
        if (targetUnits != null)
        {
            foreach (var unit in targetUnits)
            {
                HighlightTarget(unit, type, isOn);
            }
        }
    }

}
