using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IUpperUnit, IMovableUnit
{
    public BaseUnit CurrentOn { get { return _currentOn; } set { _currentOn = value; } }
    public float    Height    { get { return _heigth   ; } set { _heigth    = value; } }
    public bool     CanBeFire { get { return _canBeFire; } set { _canBeFire = value; } }
    public float    MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public bool     IsMoving  { get { return _isMoving ; } set { _isMoving  = value; } }


    #region 私有属性
    private BaseUnit _currentOn;
    private float    _heigth    = 0f;
    private bool     _canBeFire = false;
    private float    _moveSpeed = 4f;
    private bool     _isMoving  = false;

    private ENUM_UpperUnit Type = ENUM_UpperUnit.Chest;               //放置单元的类型
    private ENUM_UpperUnitControlType ControlType = ENUM_UpperUnitControlType.Movable;  //放置单元的操控类型
    private ENUM_UpperUnitBeFiredType BeFiredType = ENUM_UpperUnitBeFiredType.NULL;     //被火焰操控类型

    private BaseUnit targetUnit;
    private Vector3 targetPos;
    #endregion


    public void Init()
    {
        //初始化状态
        _currentOn.SetState(new Ground(_currentOn));
        _currentOn.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
        _currentOn.SetUpperGameObject(gameObject);
        _currentOn.SetCanBeFire(_canBeFire);


        transform.position = SetTargetPos(transform.position);
        targetPos = SetTargetPos(transform.position);
        _isMoving = false;
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


    private void Update()
    {
        MoveProcess();
    }


    /// <summary>
    /// 外部调用推动此物体的方法
    /// </summary>
    /// <param name="inputEvent"></param>
    public void Move(ENUM_InputEvent inputEvent)
    {
        if (!_isMoving)
        {
            targetUnit = GetTargetUnit(inputEvent);
            JudgeBaseStateAndMove(targetUnit);
        }
    }


    /// <summary>
    /// 判断是否可以被移动
    /// 主要被推动发起人调用
    /// </summary>
    /// <param name="inputEvent"></param>
    /// <returns></returns>
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


    /// <summary>
    /// 返回y值改为_height属性的向量
    /// </summary>
    /// <param name="_targetPos"></param>
    /// <returns></returns>
    public Vector3 SetTargetPos(Vector3 _targetPos)
    {
        return new Vector3(_targetPos.x, _heigth, _targetPos.z);
    }


    /// <summary>
    /// 处理移动的具体过程
    /// </summary>
    private void MoveProcess()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, _moveSpeed * Time.deltaTime);

        if (Vector3.Magnitude(transform.position - targetPos) < 0.1f)
        {
            _isMoving = false;
        }

    }


    /// <summary>
    /// 判断底层的状态，并移动物体
    /// </summary>
    /// <param name="targetUnit"></param>
    private void JudgeBaseStateAndMove(BaseUnit targetUnit)
    {
        if (targetUnit != null && targetUnit.CanWalk)
        {
            targetPos = SetTargetPos(targetUnit.Model.transform.position);
            if (targetUnit.State.StateType != ENUM_State.Block)
            {
                targetUnit.StateEnd();
                //将箱子走过的路径设为地面
                targetUnit.SetState(new Ground(targetUnit));
            }
            //更新目标单元的上层单元信息
            targetUnit.UpperUnit = new UpperUnit(Type, ControlType, BeFiredType);
            targetUnit.SetUpperGameObject(gameObject);
            _currentOn.UpperUnit.InitOrReset();
            _currentOn.SetUpperGameObject(null);
            _currentOn = targetUnit;
            //这里是移动的开关
            _isMoving = true;
        }

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




}
