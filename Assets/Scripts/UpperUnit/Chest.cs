using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IUpperUnit, IMovableUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float Height { get { return _heigth; } set { _heigth = value; } }
    public bool CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public bool IsMoving { get { return _isMoving; } set { _isMoving = value; } }

    //该单元的私有属性
    private BaseUnit _currentOn;
    private float _heigth = 0f;
    private bool _canBeFire = false;
    private float _moveSpeed = 4f;
    private bool _isMoving = false;

    private BaseUnit targetUnit;
    private Vector3 targetPos;


    private void Start()
    {
        Init();
    }

    private void Update()
    {
        MoveProcess();
    }

    public void Init()
    {
        //初始化状态
        _currentOn.myState.OnStateEnd();
        _currentOn.SetState(new Ground(_currentOn));
        _currentOn.SetUpperType(ENUM_UpperUnitType.Movable);
        _currentOn.SetUpperGameObject(gameObject);

        transform.position = SetTargetPos(transform.position);
        targetPos = SetTargetPos(transform.position);
        _isMoving = false;

    }

    public void Move(ENUM_InputEvent inputEvent)
    {
        if (!_isMoving)
        {
            targetUnit = GetTargetUnit(inputEvent);
            JudgeBaseStateAndMove(targetUnit);
        }
    }

    private void MoveProcess()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, _moveSpeed * Time.deltaTime);

        if (Vector3.Magnitude(transform.position - targetPos) < 0.1f)
        {
            _isMoving = false;
        }

    }

    //将单元的高度转换为模型高度
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _heigth, _targetPos.z);
    }

    private void JudgeBaseStateAndMove(BaseUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.CanWalk)
        {
            targetPos = SetTargetPos(targetUnit.Model.transform.position);
            if (targetUnit.myState.stateType != ENUM_State.Block)
            {
                targetUnit.myState.OnStateEnd();
                //将箱子走过的路径设为地面
                targetUnit.SetState(new Ground(targetUnit));
            }
            targetUnit.SetUpperType(ENUM_UpperUnitType.Movable);
            targetUnit.SetUpperGameObject(gameObject);
            _currentOn.SetUpperGameObject(null);
            _currentOn = targetUnit;
            //这里是移动的开关
            _isMoving = true;
        }

    }

    //判断是否可以被移动
    public bool JudgeCanMove(ENUM_InputEvent inputEvent)
    {
        BaseUnit targetUnit = GetTargetUnit(inputEvent);
        bool canMove = false;
        if (targetUnit != null)
        {
            if (targetUnit.UpperGameObject != null)
                canMove = false;
            else
                if (targetUnit.CanWalk)
                canMove = true;
        }
        else canMove = false;

        return canMove;
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



    public void End()
    {

    }

}
