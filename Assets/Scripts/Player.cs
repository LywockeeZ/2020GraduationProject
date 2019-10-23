using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : UpperUnit
{
    public float moveSpeed = 4f;
    public float rotateSpeed = 10f;
    //角色距离地面的高度
    public float heigth = 0.3f;

    private Vector3 targetPos;
    private Vector3 lookdir;
    private bool isMoving = false;
    private bool _canBeFire = false;


    private void Start()
    {
        Init();
    }

    private void Update()
    {
        MoveProcess();
    }

    public override void Init()
    {
        SetCanBeFire(_canBeFire);
        //初始化状态
        currentOn.myState.OnStateEnd();
        currentOn.SetState(new Block(currentOn));
        currentOn.SetUpperType(Enum.ENUM_UpperUnitType.Movable);
        currentOn.SetUpperUnit(this);

        transform.position = SetTargetPos(transform.position);
        targetPos = SetTargetPos(transform.position);
        lookdir = - transform.forward;
        //对输入事件注册
        InputManager.InputEvent += Move;
        isMoving = false;
    }

    public override void Move(Enum.ENUM_InputEvent inputEvent)
    {
        if (!isMoving)
        {
            //每次行动扣除行动点数
            GameManager.Instance.ReducePoints(1);
            switch (inputEvent)
            {
                case Enum.ENUM_InputEvent.Up:
                    ChangeState(currentOn.Up, inputEvent);
                    break;
                case Enum.ENUM_InputEvent.Down:
                    ChangeState(currentOn.Down, inputEvent);
                    break;
                case Enum.ENUM_InputEvent.Left:
                    ChangeState(currentOn.Left, inputEvent);
                    break;
                case Enum.ENUM_InputEvent.Right:
                    ChangeState(currentOn.Right, inputEvent);
                    break;
                default:
                    break;
            }
        }

    }

    //执行具体的移动过程
    private void MoveProcess()
    {
        transform.forward = Vector3.Slerp(transform.forward, lookdir, rotateSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Magnitude(transform.position - targetPos) < 0.1f)
        {
            isMoving = false;
        }
    }

    //将单元的高度转换为模型高度
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, heigth, _targetPos.z);
    }

    private Vector3 GetLookDir()
    {
        Quaternion rotation = Quaternion.FromToRotation(-transform.forward, targetPos - SetTargetPos(currentOn.Model.transform.position));
        return rotation * (-transform.forward);
    }

    //改变下一格的状态
    private void ChangeState(BaseUnit baseUnit, Enum.ENUM_InputEvent inputEvent)
    {
        if (baseUnit != null && baseUnit.CanWalk)
        {
            targetPos = SetTargetPos(baseUnit.Model.transform.position);
            lookdir = GetLookDir();
            if (baseUnit.UpperType == Enum.ENUM_UpperUnitType.Movable)
            {
                Chest chest = (Chest)baseUnit.upperUnit;
                chest.Move(inputEvent);
            }
            baseUnit.myState.OnStateEnd();
            baseUnit.SetState(new Block(baseUnit));
            baseUnit.SetUpperType(Enum.ENUM_UpperUnitType.Movable);
            currentOn.SetUpperUnit(null);
            currentOn = baseUnit;
            isMoving = true;
        }

    }
}
