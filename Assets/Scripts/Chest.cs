using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : UpperUnit
{
    public float moveSpeed = 4f;
    public float heigth = 0.3f;

    private Vector3 targetPos;
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
        currentOn.SetState(new Ground(currentOn));
        currentOn.SetUpperType(Enum.ENUM_UpperUnitType.Movable);
        currentOn.SetUpperUnit(this);

        transform.position = SetTargetPos(transform.position);
        targetPos = SetTargetPos(transform.position);
        isMoving = false;

        InputManager.InputEvent += Move;

    }

    public override void Move(Enum.ENUM_InputEvent inputEvent)
    {
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

    private void MoveProcess()
    {
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

    private void ChangeState(BaseUnit baseUnit, Enum.ENUM_InputEvent inputEvent)
    {
        if (baseUnit != null && baseUnit.CanWalk)
        {
            targetPos = SetTargetPos(baseUnit.Model.transform.position);
            baseUnit.myState.OnStateEnd();
            baseUnit.SetState(new Ground(baseUnit));
            baseUnit.SetUpperType(Enum.ENUM_UpperUnitType.Movable);
            currentOn.SetUpperUnit(null);
            currentOn = baseUnit;
            isMoving = true;
        }

    }
}
