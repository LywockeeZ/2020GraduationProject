using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IUpperUnit, IMovableUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float    Height    { get { return _heigth   ; } set { _heigth    = value; } }
    public bool     CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }
    public bool     IsMoving  { get { return _isMoving ; } set { _isMoving  = value; } }
    public float    MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }

    private ENUM_UpperUnit            Type        = ENUM_UpperUnit.Player;              //放置单元的类型
    private ENUM_UpperUnitControlType ControlType = ENUM_UpperUnitControlType.Movable;  //放置单元的操控类型
    private ENUM_UpperUnitBeFiredType BeFiredType = ENUM_UpperUnitBeFiredType.NULL;     

    //该单元的私有属性
    private BaseUnit _currentOn;
    private float _heigth = 0f;
    private bool _canBeFire = false;
    private float _moveSpeed = 4f;
    private bool _isMoving = false;

    private BaseUnit targetUnit;
    private Vector3 targetPos;
    private Vector3 lookdir;
    private float _rotateSpeed = 10f;
    private Animator m_Animator;


    private void Start()
    {
        //对输入事件注册
        InputManager.InputEvent += Move;
        m_Animator = GetComponent<Animator>();
    }


    private void Update()
    {
        if (!Game.Instance.GetCanFreeMove())
        {
            MoveProcess();

            if (_isMoving == true )
            {
                m_Animator.SetFloat("Blend", 0);
                m_Animator.SetBool("isWalking", true);
            }
            else m_Animator.SetBool("isWalking", false);

        }

    }


    public void Init()
    {
        //初始化状态
        _currentOn.State.OnStateEnd();
        _currentOn.SetState(new Block(_currentOn));
        _currentOn.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
        _currentOn.SetUpperGameObject(gameObject);
        _currentOn.SetCanBeFire(_canBeFire);

        transform.position = SetTargetPos(transform.position);
        targetPos = SetTargetPos(transform.position);
        //lookdir = - transform.forward;

        _isMoving = false;
    }


    public void Move(ENUM_InputEvent inputEvent)
    {
        if (!_isMoving)
        {
            targetUnit = GetTargetUnit(inputEvent);
            JudgeAndMove(targetUnit, inputEvent);
        }

    }

    /// <summary>
    /// 自由移动时使用，移动到目标位置
    /// </summary>
    /// <param name="m_targetPos"></param>
    public void Move(Vector3 m_targetPos)
    {
        targetPos = SetTargetPos(m_targetPos);
        _moveSpeed = 2f;
        _isMoving = true;
    }


    //执行具体的移动过程
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


    //将单元的高度转换为模型高度
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _heigth, _targetPos.z);
    }


    private Vector3 GetLookDir()
    {
        Quaternion rotation = Quaternion.FromToRotation(-transform.forward, targetPos - SetTargetPos(_currentOn.Model.transform.position));
        return rotation * (-transform.forward);
    }


    //判断下层的下一格，并改变下一格的状态
    private void JudgeBaseStateAndMove(BaseUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.CanWalk)
        {
            //当目标与当前位置都是油时，花费一点数改变为阻燃带
            if (_currentOn.State.stateType == ENUM_State.Oil)
            {
                Game.Instance.CostAP(1, 0);
                _currentOn.State.OnStateEnd();
                _currentOn.SetState(new Block(_currentOn));
            }
            else
            {
                targetPos = SetTargetPos(targetUnit.Model.transform.position);
                lookdir = GetLookDir();
                //保持在油，水，阻燃带上行走不改变状态
                if (targetUnit.State.stateType != ENUM_State.Block &&
                    targetUnit.State.stateType != ENUM_State.Oil &&
                    targetUnit.State.stateType != ENUM_State.Water)
                {
                    targetUnit.State.OnStateEnd();
                    targetUnit.SetState(new Block(targetUnit));
                }
                targetUnit.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
                targetUnit.SetUpperGameObject(gameObject);
                _currentOn.UpperUnit.InitOrReset();
                _currentOn.SetUpperGameObject(null);
                _currentOn = targetUnit;

                //扣除移动点数，根据是否是油来减速
                if (_currentOn.State.stateType == ENUM_State.Oil)
                {
                    Game.Instance.CostAP(1, 0);
                    _moveSpeed = 2f;
                }
                else Game.Instance.CostAP(1, 0);

                //这里是移动的开关
                _isMoving = true;

            }

        }
    }

    //获取目的地单元
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

    //判断的顺序为先判断是否有单元，如果有则先上层，再下层
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

    //判断上层物品是否可推动
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

    }


    public void End()
    {

    }

    public bool JudgeCanMove(ENUM_InputEvent inputEvent)
    {
        return false;
    }
}
