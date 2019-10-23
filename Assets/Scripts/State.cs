using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class State
{
    public Enum.ENUM_State stateType;
    //调试用
    public string StateName { get { return _stateName; } }
    protected string _stateName = null;
    protected BaseUnit Owner = null;

    public State(BaseUnit owner)
    {
        //设置状态拥有者
        Owner = owner;
    }

    public abstract void OnStateBegin();
    public abstract void OnStateHandle();
    public abstract void OnStateEnd();

    public Vector3 GetTargetPos(Vector3 pos, float height)
    {
        return new Vector3(pos.x, height, pos.z);
    }

}
