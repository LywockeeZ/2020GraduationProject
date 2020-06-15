using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using DG.Tweening;

public class WaterBag : SkillInstanceBase
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
    private GameObject waterBag;
    private bool isSkillStart = false;
    /// <summary>
    /// 1上2下3左4右
    /// </summary>
    private int direct = 0;

    public WaterBag(string name) : base(name)
    {
        //m_SkillTrigers.Add(SkillTriggerType.Animation, new PlayAnimationTrigger("item_Pump", 0f, OnTriggerComplete));
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 200f, 1 << LayerMask.NameToLayer("CanEmmit")))
            {
                SkillIndicator indicator = hitInfo.transform.parent.gameObject.GetComponent<SkillIndicator>();
                indicator.SetIsMouseIn(true);

                if (Input.GetMouseButtonDown(0))
                {
                    chooseUnit = hitInfo.transform.parent.parent.GetComponent<BaseUnit>();
                    indicator.HighlightCancel();
                    Debug.Log("技能施放");
                    Game.Instance.GetPlayerUnit().ExecuteSkill("item_WaterBag");
                    isEnd = true;
                    break;
                }
            }
            yield return null;
        } while (!isEnd);
        CoroutineManager.StopCoroutine(m_skillEmitterCoroutine);
    }

    public override void Execute(ISkillCore instance)
    {
        //先将技能逻辑注册给动画，再开始触发动画触发器
        WeaponController.Instance.setWaterBag += setWaterBag;
        WeaponController.Instance.throwWaterBag += throwWaterBag;
        WeaponController.Instance.EndKnife();
        base.Execute(instance);
    }

    void setWaterBag()
    {
        waterBag = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Prefabs/Items/daoju_shuidai", WeaponController.Instance.rightHandPos.position);
        waterBag.transform.forward = Game.Instance.GetPlayerUnit().transform.forward;
        waterBag.transform.SetParent(WeaponController.Instance.rightHandPos);
    }

    void throwWaterBag()
    {
        isSkillStart = true;
    }

    protected override IEnumerator SkillProcess(ISkillCore instance)
    {
        //获取路径上所有单元
        unitsOnPath = FindUnitsOnPath(chooseUnit);

        WaitForSeconds startTime =  new WaitForSeconds(m_StartTime);
        yield return startTime;

        Player player = (Player)instance.UpperUnit;
        tweeners.Add(player.transform.DOLookAt(chooseUnit.Model.transform.position, 0.5f));
        WaitForSeconds time = new WaitForSeconds(0.5f);
        yield return time;
        setWaterBag();
        throwWaterBag();
        while(!isSkillStart)
        {
            yield return null;
        }

        waterBag.transform.SetParent(null);
        tweeners.Add(waterBag.transform.GetChild(0).DOLocalMoveY(-1.3f, (unitsOnPath.Count - 1) * 0.5f).SetEase(Ease.OutBounce));
        tweeners.Add(waterBag.transform.DOMove(unitsOnPath[unitsOnPath.Count - 1].transform.position + Vector3.up*0.7f,  unitsOnPath.Count * 0.5f).
                        From(Game.Instance.GetPlayerUnit().CurrentOn.transform.position + Game.Instance.GetPlayerUnit().transform.forward * 0.2f + Vector3.up*0.7f).SetEase(Ease.OutCubic));
        WaitForSeconds skilltime = new WaitForSeconds((unitsOnPath.Count - 1) * 0.5f + 0.2f);
        yield return skilltime;

        GameObject.Destroy(waterBag);
            switch(direct)
            {
                case 1:
                    EndFire(unitsOnPath[unitsOnPath.Count - 1]);
                    EndFire(unitsOnPath[unitsOnPath.Count - 1].Left);
                    EndFire(unitsOnPath[unitsOnPath.Count - 1].Right);
                    break;
                case 2:
                    EndFire(unitsOnPath[unitsOnPath.Count - 1]);
                    EndFire(unitsOnPath[unitsOnPath.Count - 1].Up);
                    EndFire(unitsOnPath[unitsOnPath.Count - 1].Down);
                    break;
                default:
                    Debug.Log("未知的方向");
                    break;
            }
            
            OnTriggerComplete();
            CoroutineManager.StopCoroutine(m_skillProcessCoroutine);
    }

    protected override void OnSkillEnd()
    {
        WeaponController.Instance.StartKnife();
        WeaponController.Instance.setWaterBag -= setWaterBag;
        WeaponController.Instance.throwWaterBag -= throwWaterBag;
        isSkillStart = false;
        if(waterBag != null)
            GameObject.Destroy(waterBag);
        base.OnSkillEnd();
    }


    private List<BaseUnit> FindUnitsOnPath(BaseUnit chooseUnit)
    {
        List<BaseUnit> unitsOnPath = new List<BaseUnit>();
        Vector3 direction = (chooseUnit.Model.transform.position - Game.Instance.GetPlayerUnit().CurrentOn.Model.transform.position).normalized;
        chooseUnit = Game.Instance.GetPlayerUnit().CurrentOn;
        if (direction.x > 0.0001)
        {
            while(chooseUnit.Right != null && (chooseUnit.Right.UpperGameObject == null && chooseUnit.Right.State.StateType != ENUM_State.Fire))
            {
                unitsOnPath.Add(chooseUnit.Right);
                chooseUnit = chooseUnit.Right;
            }
            if (chooseUnit.Right?.State.StateType == ENUM_State.Fire)
            {
                unitsOnPath.Add(chooseUnit.Right);
                chooseUnit = chooseUnit.Right;
            }
            direct = 2;
            return unitsOnPath;
        }
        else
        if (direction.x < -0.0001)
        {
            while (chooseUnit.Left != null && (chooseUnit.Left.UpperGameObject == null && chooseUnit.Left.State.StateType != ENUM_State.Fire))
            {
                unitsOnPath.Add(chooseUnit.Left);
                chooseUnit = chooseUnit.Left;
            }
            if (chooseUnit.Left?.State.StateType == ENUM_State.Fire)
            {
                unitsOnPath.Add(chooseUnit.Left);
                chooseUnit = chooseUnit.Left;
            }
            direct = 2;
            return unitsOnPath;
        }

        if (direction.z > 0.0001)
        {
            while (chooseUnit.Up != null && (chooseUnit.Up.UpperGameObject == null && chooseUnit.Up.State.StateType != ENUM_State.Fire))
            {
                unitsOnPath.Add(chooseUnit.Up);
                chooseUnit = chooseUnit.Up;
            }
            if (chooseUnit.Up?.State.StateType == ENUM_State.Fire)
            {
                unitsOnPath.Add(chooseUnit.Up);
                chooseUnit = chooseUnit.Up;
            }
            direct = 1;
            return unitsOnPath;
        }
        else
        if (direction.z < -0.0001)
        {
            while (chooseUnit.Down != null && (chooseUnit.Down.UpperGameObject == null && chooseUnit.Down.State.StateType != ENUM_State.Fire))
            {
                unitsOnPath.Add(chooseUnit.Down);
                chooseUnit = chooseUnit.Down;
            }
            if (chooseUnit.Down?.State.StateType == ENUM_State.Fire)
            {
                unitsOnPath.Add(chooseUnit.Down);
                chooseUnit = chooseUnit.Down;
            }
            direct = 1;
            return unitsOnPath;
        }

        Debug.LogError("未正确比较出技能WaterBag选择的方向");
        return unitsOnPath;
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
        HighlightTarget(baseUnit?.Up, type, isOn);
        HighlightTarget(baseUnit?.Right, type, isOn);
        HighlightTarget(baseUnit?.Down, type, isOn);
        HighlightTarget(baseUnit?.Left, type, isOn);
    }

    private void EndFire(BaseUnit unit)
    {
        if (unit != null && unit.UpperGameObject == null)
        {
            unit.SetState(new Water(unit));
        }
    }
}
