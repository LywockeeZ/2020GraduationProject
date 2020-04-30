using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Player : MonoBehaviour, IUpperUnit, IMovableUnit, ISkillCore
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float    Height    { get { return _heigth   ; } set { _heigth    = value; } }
    public bool     CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }
    public bool     IsMoving  { get { return _isMoving ; } set { _isMoving  = value; } }
    public float    MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }

    public IUpperUnit UpperUnit { get => this; }
    public Animator SkillAnimator { get => m_Animator;  }
    public BaseUnit TargetUnit { get => targetUnit; }
    public NavMeshAgent Agent { get => m_Agent; }


    #region 私有字段
    private BaseUnit _currentOn;
    private float _heigth = 0f;
    private bool _canBeFire = false;
    private float _moveSpeed = 4f;
    private bool _isMoving = false;

    private ENUM_UpperUnit Type = ENUM_UpperUnit.Player;              //放置单元的类型
    private ENUM_UpperUnitControlType ControlType = ENUM_UpperUnitControlType.Movable;  //放置单元的操控类型
    private ENUM_UpperUnitBeFiredType BeFiredType = ENUM_UpperUnitBeFiredType.NULL;

    //私有变量
    private BaseUnit targetUnit;
    private Vector3 targetPos;
    private Vector3 lookdir;
    private float _rotateSpeed = 10f;
    private Animator m_Animator;
    private NavMeshAgent m_Agent;
    private LocalNavMeshBuilder m_NavMeshBuder;
    private bool isRandomIdle = false;
    #endregion


    public void Init()
    {
        //初始化状态
        _currentOn.SetState(new Block(_currentOn));
        _currentOn.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
        _currentOn.SetUpperGameObject(gameObject);
        _currentOn.SetCanBeFire(_canBeFire);

        if (!Game.Instance.GetCanFreeMove())
        {
            transform.forward = Vector3.forward;
            m_Agent.updatePosition = true;
            m_NavMeshBuder.StartUpdateNavMesh();
            m_Agent.Warp(SetTargetPos(_currentOn.Model.transform.position));
        }
        else
        {
            //自由移动模式进入关卡时先移动到初始位置
            //MoveByNavMesh(SetTargetPos(_currentOn.Model.transform.position));
            m_Agent.Warp(SetTargetPos(_currentOn.Model.transform.position));
            Game.Instance.SetCanFreeMove(false);
        }



        _isMoving = false;

        Debug.Log("Player已初始化");

    }


    public void End()
    {
        m_Agent.updatePosition = false;
        if (CurrentOn != null)
        {
            CurrentOn.UpperGameObject = null;
        }
        gameObject.SetActive(false);
    }


    private void Start()
    {
        //对输入事件注册
        InputManager.InputEvent += Move;
        m_Animator = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_NavMeshBuder = GetComponent<LocalNavMeshBuilder>();

        Game.Instance.SetPlayerUnit(this);
    }


    private void Update()
    {
        //if (!Game.Instance.GetCanFreeMove())
        //{
        //    //MoveProcess();

        //    if (_isMoving == true )
        //    {
        //        m_Animator.SetFloat("Blend", 0);
        //        m_Animator.SetBool("isWalking", true);
        //    }
        //    else m_Animator.SetBool("isWalking", false);

        //}

        //处理移动的判断和动画
        if (_isMoving)
        {
            m_Animator.SetBool("isWalking", true);

            if ((transform.position - targetPos).magnitude <= 0.5f)
            {
                //Debug.Log("Have Reached!");
                _isMoving = false;
            }
        }
        else m_Animator.SetBool("isWalking", false);

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle"))
        {
            if (!isRandomIdle)
            {
                StartCoroutine(RandomIdle());
                isRandomIdle = true;
            }
        }
        else StopCoroutine(RandomIdle());

    }

    IEnumerator RandomIdle()
    {
        float waitTime = UnityEngine.Random.Range(15, 25);
        int state = UnityEngine.Random.Range(1, 3);
        
        yield return new WaitForSeconds(waitTime);
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Player_idle"))
        {
            switch (state)
            {
                case 1:
                    m_Animator.SetTrigger("idle1");
                    isRandomIdle = false;
                    break;
                case 2:
                    m_Animator.SetTrigger("idle2");
                    isRandomIdle = false;
                    break;
                default:
                    isRandomIdle = false;
                    break;
            }
        }
        else
            isRandomIdle = false;
    }

    /// <summary>
    /// 用来判断移动，判断是否可移动，并不执行具体的移动
    /// </summary>
    /// <param name="inputEvent"></param>
    public void Move(ENUM_InputEvent inputEvent)
    {
        if (!_isMoving)
        {
            targetUnit = GetTargetUnit(inputEvent);
            JudgeAndMove(targetUnit, inputEvent);
        }

    }


    /// <summary>
    /// 利用NavmeshAgent的移动
    /// 移动到目标位置
    /// </summary>
    /// <param name="m_targetPos"></param>
    public void MoveByNavMesh(Vector3 m_targetPos)
    {
        //关卡中移动时扣除点数
        if (!Game.Instance.GetCanFreeMove())
            Game.Instance.CostAP(1, 0);

        m_Agent.SetDestination(m_targetPos);
        targetPos = m_targetPos;
        _isMoving = true;
        Game.Instance.NotifyEvent(ENUM_GameEvent.PlayerMove);
    }


    /// <summary>
    /// 将单元的高度转换为模型高度
    /// </summary>
    /// <param name="_targetPos"></param>
    /// <returns></returns>
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _targetPos.y + _heigth, _targetPos.z);
    }


    /// <summary>
    /// 执行具体的移动过程
    /// </summary>
    private void MoveProcess()
    {
        transform.forward = Vector3.Slerp(transform.forward, lookdir, _rotateSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPos, _moveSpeed * Time.deltaTime);

        if (Vector3.Magnitude(transform.position - targetPos) < 0.1f)
        {
            _isMoving = false;
            _moveSpeed = 4f;
        }
    }


    private Vector3 GetLookDir()
    {
        Quaternion rotation = Quaternion.FromToRotation(-transform.forward, targetPos - SetTargetPos(_currentOn.Model.transform.position));
        return rotation * (-transform.forward);
    }


    /// <summary>
    /// 判断下层的下一格，并改变下一格的状态
    /// </summary>
    /// <param name="targetUnit"></param>
    private void JudgeBaseStateAndMove(BaseUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.CanWalk)
        {
            //当目标与当前位置都是油时，花费一点数改变为阻燃带
            if (_currentOn.State.StateType == ENUM_State.Oil)
            {
                Game.Instance.CostAP(1, 0);
                _currentOn.SetState(new Block(_currentOn));
            }
            else
            {

                ////////////////////////////////////////////////////////////////
                //旧版移动相关
                //targetPos = SetTargetPos(targetUnit.Model.transform.position);
                //lookdir = GetLookDir();
                ////////////////////////////////////////////////////////////////
                

                //保持在油，水，阻燃带上行走不改变状态
                if (targetUnit.State.StateType == ENUM_State.Ground)
                    targetUnit.SetState(new Block(targetUnit));

                UpdateUnit(targetUnit);

                //用来移动的方法
                if (targetUnit.State.StateType == ENUM_State.Fire)
                {
                    ExecuteSkill("skill_NormalAttack");
                }
                else  MoveByNavMesh(targetUnit.Model.transform.position);

            }

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
        _currentOn.UpperUnit.InitOrReset();
        _currentOn.SetUpperGameObject(null);
        _currentOn = targetUnit;
    }


    /// <summary>
    /// 获取目的地单元
    /// </summary>
    /// <param name="inputEvent"></param>
    /// <returns></returns>
    private BaseUnit GetTargetUnit(ENUM_InputEvent inputEvent)
    {
        BaseUnit targetUnit = null;
        switch (inputEvent)
        {
            case ENUM_InputEvent.Up:
                    targetUnit = _currentOn.Up;
                break;
            case ENUM_InputEvent.Down:
                    targetUnit = _currentOn.Down;
                break;
            case ENUM_InputEvent.Left:
                    targetUnit = _currentOn.Left;
                break;
            case ENUM_InputEvent.Right:
                    targetUnit = _currentOn.Right;
                break;
            default:
                break;
        }

        return targetUnit;
    }


    /// <summary>
    /// 判断的顺序为先判断是否有单元，如果有则先上层，再下层
    /// </summary>
    /// <param name="targetUnit"></param>
    /// <param name="inputEvent"></param>
    private void JudgeAndMove(BaseUnit targetUnit, ENUM_InputEvent inputEvent)
    {
        if (targetUnit != null)
        {
            if (targetUnit.UpperGameObject != null)
                JudgeUpperUnit(targetUnit, inputEvent);
            else
            {
                JudgeBaseStateAndMove(targetUnit);
            }

        }
    }


    /// <summary>
    /// 判断上层物品是否可推动
    /// </summary>
    /// <param name="targetUnit"></param>
    /// <param name="inputEvent"></param>
    private void JudgeUpperUnit(BaseUnit targetUnit, ENUM_InputEvent inputEvent)
    {
        if (targetUnit.UpperUnit.ControlType == ENUM_UpperUnitControlType.Movable)
        {
            IMovableUnit movableUnit = targetUnit.UpperGameObject.GetComponent<IMovableUnit>();

            //判断被推动物体是否可以前进
            if (movableUnit.JudgeCanMove(inputEvent))
            {
                movableUnit.Move(inputEvent);
                JudgeBaseStateAndMove(targetUnit);
            }
        }
        else
        if (targetUnit.UpperUnit.ControlType == ENUM_UpperUnitControlType.Fixed)
        {
            IFixedUnit fixedUnit = targetUnit.UpperGameObject.GetComponent<IFixedUnit>();

            if (fixedUnit != null)
            {
                fixedUnit.Handle();
            }
        }
        else JudgeBaseStateAndMove(targetUnit);

    }


    public bool JudgeCanMove(ENUM_InputEvent inputEvent)
    {
        return false;
    }


    /// <summary>
    /// 执行主技能
    /// </summary>
    public void ExecuteSkill()
    {
        Game.Instance.GetMainSkill().Execute(this);
    }

    public void ExecuteSkill(string skillName)
    {
        Game.Instance.GetSkill(skillName).Execute(this);
    }

    /// <summary>
    /// 技能事件调用
    /// </summary>
    public void OnAnimationEnd()
    {
        //PlayAnimationTrigger animationTrigger = (PlayAnimationTrigger)Game.Instance.GetMainSkill()?.m_SkillTrigers[SkillTriggerType.Animation];
        //animationTrigger.OnAnimationEnd();
    }

}
