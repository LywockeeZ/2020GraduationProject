using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OilTank : MonoBehaviour, IUpperUnit, IFixedUnit, ICanBeFiredUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float    Height    { get { return _heigth   ; } set { _heigth    = value; } }
    public bool     CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }

    public Animator animator;


    #region 私有属性
    private ENUM_UpperUnit Type = ENUM_UpperUnit.OilTank;                             //放置单元的类型
    private ENUM_UpperUnitControlType ControlType = ENUM_UpperUnitControlType.Fixed;  //放置单元的操控类型
    private ENUM_UpperUnitBeFiredType BeFiredType = ENUM_UpperUnitBeFiredType.BeFire; 

    private BaseUnit _currentOn;
    private float    _heigth = 0f;
    private bool     _canBeFire = true;
    private GameObject explosionEffectByFire;
    private GameObject explosionEffectByHand;
    #endregion


    public void Init()
    {
        //初始化状态
        _currentOn.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
        _currentOn.SetUpperGameObject(gameObject);
        _currentOn.SetCanBeFire(_canBeFire);

        transform.position = SetTargetPos(transform.position);
        transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        animator = transform.GetChild(0).GetComponent<Animator>();

        explosionEffectByHand = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/OilTankExplosionHand", transform.position);
        explosionEffectByFire = GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/OilTankExplosionFire", transform.position);
        explosionEffectByHand.transform.forward = Vector3.up;
        explosionEffectByFire.transform.forward = Vector3.up;
    }


    public void End()
    {
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(this.gameObject);
        _currentOn.UpperUnit.InitOrReset();
        CurrentOn.UpperGameObject = null;
        Destroy(this);
    }


    private void Start()
    {
        Init();
    }


    /// <summary>
    /// 玩家控制触发调用的方法
    /// </summary>
    public void Handle(bool isCost = true)
    {
        explosionEffectByHand.GetComponent<ParticleSystem>().Play();

        Player player = Game.Instance.GetPlayerUnit();
        if (Game.Instance.GetExecutingSkill() == null)
        {
            player.transform.DOLookAt(CurrentOn.Model.transform.position, 0.3f);
        }

        if (isCost)
            Game.Instance.CostAP(1, 0);

        _currentOn.StateEnd();
        _currentOn.SetState(new Oil(_currentOn));

        SetAroundToOil();
        //if (animator != null) animator.SetTrigger("Break");

        End();
    }


    /// <summary>
    /// 火焰触发调用的方法
    /// </summary>
    public void HandleByFire()
    {
        explosionEffectByFire.GetComponent<ParticleSystem>().Play();

        _currentOn.SetState(new Fire(_currentOn));
        CoroutineManager.StartCoroutine(BoomAccess());

        if (animator != null) animator.SetTrigger("Break");
        End();

    }


    /// <summary>
    /// 返回y为_height的向量
    /// </summary>
    /// <param name="_targetPos"></param>
    /// <returns></returns>
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _targetPos.y + _heigth, _targetPos.z);
    }


    /// <summary>
    /// 将围绕单元一圈范围设置为油
    /// </summary>
    private void SetAroundToOil()
    {
        List<BaseUnit> units = GetBoomRangeAround();
        if (CurrentOn.Up != null )
        {
            SetTargetToOil(units[0]);
            SetTargetToOil(units[1]);
            SetTargetToOil(units[2]);
        }
        if (CurrentOn.Down != null )
        {
            SetTargetToOil(units[5]);
            SetTargetToOil(units[6]);
            SetTargetToOil(units[7]);
        }
        if (CurrentOn.Left != null )
        {
            SetTargetToOil(units[0]);
            SetTargetToOil(units[3]);
            SetTargetToOil(units[5]);
        }
        if (CurrentOn.Right != null )
        {
            SetTargetToOil(units[2]);
            SetTargetToOil(units[4]);
            SetTargetToOil(units[7]);
        }


    }


    /// <summary>
    /// 将目标单元设置为油
    /// </summary>
    /// <param name="targetUnit"></param>
    private void SetTargetToOil(BaseUnit targetUnit)
    {
        if (targetUnit != null &&
            (targetUnit.State.StateType == ENUM_State.Ground || targetUnit.State.StateType == ENUM_State.Block))
        {
            targetUnit.StateEnd();
            targetUnit.SetState(new Oil(targetUnit));
        }
    }


    /// <summary>
    /// 将目标单元设置为火
    /// </summary>
    /// <param name="targetUnit"></param>
    private void SetTargetToFire(BaseUnit targetUnit)
    {
        if (targetUnit != null)
        {
            if (targetUnit.UpperUnit.Type != ENUM_UpperUnit.NULL)
            {
                if (targetUnit.UpperUnit.BeFiredType != ENUM_UpperUnitBeFiredType.BeFire)
                {
                    return;
                }
                else
                if (targetUnit.UpperUnit.BeFiredType == ENUM_UpperUnitBeFiredType.BeFire)
                {
                    targetUnit.UpperGameObject.GetComponent<ICanBeFiredUnit>().HandleByFire();
                }
                else
                if (targetUnit.UpperUnit.Type == ENUM_UpperUnit.Player)
                {
                    targetUnit.StateEnd();
                    targetUnit.SetState(new Block(targetUnit));
                }
            }
            else
            {
                if (targetUnit.State.BeFiredType == ENUM_StateBeFiredType.BeFire)
                {
                    targetUnit.StateEnd();
                    targetUnit.SetState(new Fire(targetUnit));
                }
                else
                if (targetUnit.State.BeFiredType == ENUM_StateBeFiredType.BeHandle)
                {
                    targetUnit.StateRequest();
                }
            }

        }
    }


    /// <summary>
    /// 油桶爆炸函数
    /// </summary>
    private void Boom()
    {
        List<BaseUnit> units = GetBoomRangeAround();
        foreach (var unit in units)
        {
            SetTargetToFire(unit);
        }
    }


    /// <summary>
    /// 对爆炸外围波及到的火焰蔓延
    /// </summary>
    private void FireSpread()
    {
        List<BaseUnit> units = GetBoomRangeAdditionAround();
        List<BaseUnit> FireNeighbor = new List<BaseUnit>();
        foreach (var unit in units)
        {
            if (unit != null)
            {
                if (unit.UpperUnit.Type == ENUM_UpperUnit.NULL)
                {
                    if (unit.State.StateType == ENUM_State.Fire)
                    {
                        FireNeighbor.Add(unit);
                    }
                }
            }
        }
        foreach (var unit in FireNeighbor)
        {
            unit.StateRequest();
        }
        _currentOn.GetStage().isOilUpdateEnd = true;
    }


    /// <summary>
    /// 获取爆炸范围内的信息
    /// </summary>
    /// <returns></returns>
    private List<BaseUnit> GetBoomRangeAround()
    {
        NormalStageData _currentStage = CurrentOn.GetStage();
        List<BaseUnit> units = new List<BaseUnit>();
        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x - 1]);
        else units.Add(null);
        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x    ]);
        else units.Add(null);
        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x + 1]);
        else units.Add(null);
        if (CurrentOn.y     >= 0 && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x - 1]);
        else units.Add(null);
        if (CurrentOn.y     >= 0 && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x + 1]);
        else units.Add(null);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x - 1]);
        else units.Add(null);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x    ]);
        else units.Add(null);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x + 1]);
        else units.Add(null);
        return units;
    }


    /// <summary>
    /// 获取爆炸外围的信息
    /// </summary>
    /// <returns></returns>
    private List<BaseUnit> GetBoomRangeAdditionAround()
    {
        NormalStageData _currentStage = CurrentOn.GetStage();
        List<BaseUnit> units = new List<BaseUnit>();
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x - 2]);
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x - 1]);
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x    ]);
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x + 1]);
        if (CurrentOn.y + 2 < _currentStage.Row && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 2) + CurrentOn.x + 2]);

        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x - 2]);
        if (CurrentOn.y + 1 < _currentStage.Row && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y + 1) + CurrentOn.x + 2]);

        if (CurrentOn.y     < _currentStage.Row && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x - 2]);
        if (CurrentOn.y     < _currentStage.Row && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y    ) + CurrentOn.x + 2]);

        if (CurrentOn.y - 1 >= 0 && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x - 2]);
        if (CurrentOn.y - 1 >= 0 && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 1) + CurrentOn.x + 2]);

        if (CurrentOn.y - 2 >= 0 && CurrentOn.x - 2 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x - 2]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x - 1 >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x - 1]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x     >= 0)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x    ]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x + 1 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x + 1]);
        if (CurrentOn.y - 2 >= 0 && CurrentOn.x + 2 < _currentStage.Column)
            units.Add(_currentStage.baseUnits[_currentStage.Column * (CurrentOn.y - 2) + CurrentOn.x + 2]);

        return units;
    }


    /// <summary>
    /// 爆炸的过程
    /// </summary>
    /// <returns></returns>
    IEnumerator BoomAccess()
    {
        _currentOn.GetStage().isOilUpdateEnd = false;
        yield return new WaitForSeconds(0.1f);
        Boom();
        yield return new WaitForSeconds(1f);
        FireSpread();
        CoroutineManager.StopCoroutine(BoomAccess());
    }

}
