using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovableUnit
{
    float MoveSpeed { get; set; }
    bool IsMoving { get; set; }

    void Move(ENUM_InputEvent inputEvent);

    bool JudgeCanMove(ENUM_InputEvent inputEvent);
}
