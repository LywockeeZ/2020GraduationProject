using System.Collections;
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
    /// <summary>
    /// 路径尽头相邻的单元
    /// </summary>
    private BaseUnit pathNextUnit;

    private GameObject skillEffect = null;

    public Pump(string name) : base(name)
    {
        m_SkillTrigers.Add(SkillTriggerType.Animation, new PlayAnimationTrigger("item_Pump", 0f, OnTriggerComplete));
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
                    Game.Instance.GetPlayerUnit().ExecuteSkill("item_Pump");
                    isEnd = true;
                    break;
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
        Game.Instance.GetPlayerUnit().transform.forward = unitsOnPath[unitsOnPath.Count - 1].transform.position - Game.Instance.GetPlayerUnit().CurrentOn.transform.position;
        WeaponController.Instance.firePos.forward = Game.Instance.GetPlayerUnit().transform.forward;
        skillEffect = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/Laser Toon Water", WeaponController.Instance.firePos.position);
        skillEffect.GetComponent<Hovl_Laser2>().MaxLength = unitsOnPath.Count;
        skillEffect.transform.rotation =Game.Instance.GetPlayerUnit().transform.rotation;
        skillEffect.transform.GetChild(0).DOMove(skillEffect.transform.position + unitsOnPath.Count * skillEffect.transform.forward, unitsOnPath.Count * 0.2f).SetEase(Ease.Linear);
        do
        {
            unitsOnPath[0].SetState(new Water(unitsOnPath[0]));
            unitsOnPath.RemoveAt(0);
            
            if(unitsOnPath.Count == 0)
            {
                if (pathNextUnit != null && pathNextUnit.UpperUnit.Type != ENUM_UpperUnit.Bee)
                {
                    pathNextUnit.UpperGameObject.GetComponent<IFixedUnit>().Handle(false);
                    pathNextUnit = null;
                }
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
            if (chooseUnit.Right?.UpperGameObject != null)
            {
                pathNextUnit = chooseUnit.Right;
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
            if (chooseUnit.Left?.UpperGameObject != null)
            {
                pathNextUnit = chooseUnit.Left;
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
            if (chooseUnit.Up?.UpperGameObject != null)
            {
                pathNextUnit = chooseUnit.Up;
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
            if (chooseUnit.Down?.UpperGameObject != null)
            {
                pathNextUnit = chooseUnit.Down;
            }
            return unitsOnPath;
        }

        Debug.LogError("未正确比较出技能Pump选择的方向");
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
}
