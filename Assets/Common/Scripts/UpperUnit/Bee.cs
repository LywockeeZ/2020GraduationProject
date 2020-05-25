using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bee : MonoBehaviour, IUpperUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float Height { get { return _heigth; } set { _heigth = value; } }
    public bool CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }

    public NavMeshAgent Agent { get => m_Agent; }


    #region 私有属性
    private ENUM_UpperUnit Type = ENUM_UpperUnit.Bee;                             //放置单元的类型
    private ENUM_UpperUnitControlType ControlType = ENUM_UpperUnitControlType.NULL;  //放置单元的操控类型
    private ENUM_UpperUnitBeFiredType BeFiredType = ENUM_UpperUnitBeFiredType.NULL;

    private BaseUnit _currentOn;
    private float _heigth = 0f;
    private bool _canBeFire = false;
    private NavMeshAgent m_Agent;
    private LocalNavMeshBuilder m_NavMeshBuder;
    private int currentDirection = 0;
    private int failTimes = 0;
    #endregion


    public void Init()
    {
        //初始化状态
        _currentOn.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
        _currentOn.SetUpperGameObject(gameObject);
        _currentOn.SetCanBeFire(_canBeFire);

        transform.position = SetTargetPos(transform.position);
        m_NavMeshBuder.StartUpdateNavMesh();
    }


    public void End()
    {
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(this.gameObject);
        _currentOn.UpperUnit.InitOrReset();
        CurrentOn.UpperGameObject = null;
        Destroy(this);
    }

    private EventListenerDelegate OnPlayerMove;
    public void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.PlayerMove, OnPlayerMove = (Message evt) =>
        {
            MoveToNext();
        });
    }

    public void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.PlayerMove, OnPlayerMove);
    }

    private void OnEnable()
    {
        RegisterEvent();
        failTimes = 0;
        currentDirection = 0;
    }

    private void OnDisable()
    {
        DetachEvent();    
    }

    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_NavMeshBuder = GetComponent<LocalNavMeshBuilder>();

        Init();
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
    /// 利用NavmeshAgent的移动
    /// 移动到目标位置
    /// </summary>
    /// <param name="m_targetPos"></param>
    public void MoveByNavMesh(Vector3 m_targetPos)
    {
        void action()
        {
            m_Agent.SetDestination(m_targetPos);
        }
        CoroutineManager.StartCoroutineTask(action, 0.5f);
    }

    public void MoveToNext()
    {
        if (failTimes == 4)
            return;
        if (ExecuteMove(currentDirection))
        {
            failTimes = 0;
            currentDirection = (currentDirection + 1) % 4;
        }
        else
        {
            failTimes++;
            currentDirection = (currentDirection + 1) % 4;
            MoveToNext();
        }
    }

    private bool ExecuteMove(int direction)
    {
        switch (direction)
        {
            case 0:
                return CheckUnitAndMove(CurrentOn.Right);
            case 1:
                return CheckUnitAndMove(CurrentOn.Up);
            case 2:
                return CheckUnitAndMove(CurrentOn.Left);
            case 3:
                return CheckUnitAndMove(CurrentOn.Down);
            default:
                Debug.LogError("Direction Error");
                return false;
        }
    }

    private bool CheckUnitAndMove(BaseUnit unit)
    {
        if (unit != null && unit.State.StateType != ENUM_State.Fire)
        {
            MoveByNavMesh(unit.Model.transform.position);
            UpdateUnit(unit);
            return true;
        }
        else return false;
    }

    /// <summary>
    /// 更新目标单元和当前单元的信息,将当前单元换为参数所给单元
    /// </summary>
    /// <param name="targetUnit"></param>
    private void UpdateUnit(BaseUnit targetUnit)
    {
        targetUnit.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
        targetUnit.SetUpperGameObject(gameObject);
        targetUnit.SetCanBeFire(CanBeFire);
        _currentOn.UpperUnit.InitOrReset();
        _currentOn.SetUpperGameObject(null);
        _currentOn = targetUnit;
        _currentOn.SetCanBeFire(_currentOn.State.CanBeFire);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Game.Instance.ShowUI("EndStageUI");
            //Game.Instance.UIShowMessag("EndStageUI", "你死了！");
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageEnd, 1);
        }
    }
}
