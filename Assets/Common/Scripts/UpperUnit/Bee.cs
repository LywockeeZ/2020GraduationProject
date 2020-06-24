using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BeeMoveMode
{
    /// <summary>
    /// 四格方块内
    /// </summary>
    Round4, 
    /// <summary>
    /// 横向四格
    /// </summary>
    Row4,   
    /// <summary>
    /// 竖向四格
    /// </summary>
    Column4 
}
public class Bee : MonoBehaviour, IUpperUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float Height { get { return _heigth; } set { _heigth = value; } }
    public bool CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }

    public NavMeshAgent Agent { get => m_Agent; }

    public BeeMoveMode MoveMode = BeeMoveMode.Round4;


    #region 私有属性
    private ENUM_UpperUnit Type = ENUM_UpperUnit.Bee;                             //放置单元的类型
    private ENUM_UpperUnitControlType ControlType = ENUM_UpperUnitControlType.NULL;  //放置单元的操控类型
    private ENUM_UpperUnitBeFiredType BeFiredType = ENUM_UpperUnitBeFiredType.NULL;

    private BaseUnit _currentOn;
    private float _heigth = 0f;
    private bool _canBeFire = false;
    private NavMeshAgent m_Agent;
    private LocalNavMeshBuilder m_NavMeshBuder;
    /// <summary>
    /// 从起点顺时针
    /// </summary>
    private int[] round4DirectNormal   = new int[] { 0, 1, 2, 3 };
    /// <summary>
    /// 从起点逆时针
    /// </summary>
    private int[] round4DirectReverse  = new int[] { 1, 0, 3, 2 };
    /// <summary>
    /// 从起点向右
    /// </summary>
    private int[] row4DirectNormal     = new int[] { 1, 1, 1, 1};
    /// <summary>
    /// 从起点向左
    /// </summary>
    private int[] row4DirectReverse    = new int[] { 3, 3, 3, 3};
    /// <summary>
    /// 从起点向上
    /// </summary>
    private int[] column4DirectNormal  = new int[] { 0, 0, 0, 0};
    /// <summary>
    /// 从起点向下
    /// </summary>
    private int[] column4DirectReverse = new int[] { 2, 2, 2, 2};
    private int currentDirection = 0;
    private int steps = 0;
    private int index = 0;
    private int failTimes = 0;
    /// <summary>
    /// 是否开始进入反向模式
    /// </summary>
    private bool isReverse = false;
    /// <summary>
    /// 是否开始后退
    /// </summary>
    private bool isBack = false;
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

    public void SetMoveMode(BeeMoveMode mode = BeeMoveMode.Round4)
    {
        MoveMode = mode;
        currentDirection = InitDirection();
        void action()
        {
            gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
        }
        CoroutineManager.StartCoroutineTask(action, 0.2f);
    }


    public void End()
    {
        GameFactory.GetAssetFactory().DestroyGameObject<GameObject>(this.gameObject);
        _currentOn.UpperUnit.InitOrReset();
        CurrentOn.UpperGameObject = null;
        Destroy(this);
    }

    private EventListenerDelegate OnPlayerMove;
    private EventListenerDelegate OnStageEnd;
    public void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.PlayerMove, OnPlayerMove = (Message evt) =>
        {
            MoveToNext();
        });
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageEnd, OnStageEnd = (Message evt) =>
        {
            gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
        });

    }

    public void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.PlayerMove, OnPlayerMove);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageEnd, OnStageEnd);
    }

    private void OnEnable()
    {
        RegisterEvent();
        failTimes = 0;
        currentDirection = InitDirection();
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
        {
            failTimes = 0;
            return;
        }
        //执行移动并计算下次移动的方向
        if (ExecuteMove(currentDirection))
        {
            failTimes = 0;

            //控制方向数组下标的前后移动
            if (isBack)
            {
                //如果方向下表不是0则后退，否则后退结束
                if (index != 0)
                    index--;
                else
                {
                    isBack = false;
                    isReverse = !isReverse;
                }
            }
            else
            {
                if (steps == 3)
                {
                    isReverse = !isReverse;
                    steps = 0;
                }
                else
                    index = (index + 1) % 4;
            }
            currentDirection = CalculateDirection(index);
        }
        else
        {
            failTimes++;
            //控制方向数组下标的前后移动
            if (isBack)
            {
                //如果方向下表不是0则后退，否则后退结束
                if (index != 0)
                    index--;
                else
                {
                    isBack = false;
                    isReverse = !isReverse;
                }
            }
            else index = (index + 1) % 4;

            currentDirection = CalculateDirection(index);

            MoveToNext();
        }
    }

    private int InitDirection()
    {
        switch (MoveMode)
        {
            case BeeMoveMode.Round4:
                return 0;
            case BeeMoveMode.Row4:
                steps = 0;
                return 1;
            case BeeMoveMode.Column4:
                steps = 0;
                return 0;
            default:
                Debug.LogError("BeeMoveMode Error");
                return 0;
        }
    }

    private int CalculateDirection(int index)
    {
        switch (MoveMode)
        {
            case BeeMoveMode.Round4:
                if (!isReverse)
                {
                    if (!isBack)
                        //正向走
                        return round4DirectNormal[index];
                    else
                        //原路倒退
                        return (round4DirectNormal[index] + 2) % 4;
                }
                else
                {
                    if (!isBack)
                        return round4DirectReverse[index];
                    else
                        return (round4DirectReverse[index] + 2) % 4;
                }
            case BeeMoveMode.Row4:
                if (!isReverse)
                {
                    if (!isBack)
                    //正向走
                    {
                        return row4DirectNormal[index];
                    }
                    else
                    //原路倒退
                    {
                        return (row4DirectNormal[index] + 2) % 4;
                    }
                }
                else
                {
                    if (!isBack)
                    {
                        return row4DirectReverse[index];
                    }
                    else
                    {
                        return (row4DirectReverse[index] + 2) % 4;
                    }
                }
            case BeeMoveMode.Column4:
                if (!isReverse)
                {
                    if (!isBack)
                        //正向走
                        return column4DirectNormal[index];
                    else
                        //原路倒退
                        return (column4DirectNormal[index] + 2) % 4;
                }
                else
                {
                    if (!isBack)
                        return column4DirectReverse[index];
                    else
                        return (column4DirectReverse[index] + 2) % 4;
                }
            default:
                Debug.LogError("BeeMoveMode Error");
                return 0;
        }
    }

    private bool ExecuteMove(int direction)
    {
        switch (direction)
        {
            case 0:
                return CheckUnitAndMove(CurrentOn.Up);
            case 1:
                return CheckUnitAndMove(CurrentOn.Right);
            case 2:
                return CheckUnitAndMove(CurrentOn.Down);
            case 3:
                return CheckUnitAndMove(CurrentOn.Left);
            default:
                Debug.LogError("Direction Error");
                return false;
        }
    }

    private bool CheckUnitAndMove(BaseUnit unit)
    {
        if (unit != null)
        {
            if ((unit.UpperGameObject != null && unit.UpperUnit.Type == ENUM_UpperUnit.Player) ||
                (unit.UpperGameObject == null && unit.State.StateType != ENUM_State.Fire))
            {
                MoveByNavMesh(unit.Model.transform.position);
                UpdateUnit(unit);
                steps++;
                return true;
            }
            else
            {
                if (isBack)
                {
                    isReverse = !isReverse;
                }
                steps = 0;
                isBack = !isBack;
                return false;
            }

        }
        else 
        {
            if (isBack)
            {
                isReverse = !isReverse;
            }
            steps = 0;
            isBack = !isBack;
            return false; 
        }
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
            GameFactory.GetAssetFactory().InstantiateGameObject<GameObject>("Effects/TouchBee", CurrentOn.transform.position + 0.8f * Vector3.up);
            Game.Instance.NotifyEvent(ENUM_GameEvent.StageEnd, 1);
        }
    }
}
