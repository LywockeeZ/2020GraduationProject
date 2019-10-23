using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperUnit :MonoBehaviour
{
    protected BaseUnit currentOn = null;
    public bool CanBeFire { get { return _canBeFire; } }
    private bool _canBeFire;

    public virtual void SetCurrentOnUnit(BaseUnit baseUnit)
    {
        currentOn = baseUnit;
    }

    public virtual void Init() { }

    public virtual void Move(Enum.ENUM_InputEvent inputEvent) { }

    public virtual void End() { }

    public void SetCanBeFire(bool value)
    {
        _canBeFire = value;
    }


}
