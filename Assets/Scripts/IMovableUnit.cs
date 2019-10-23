using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovableUnit
{
    float MoveSpeed { get; set; }
    bool IsMoving { get; set; }

    void Move(Enum.ENUM_InputEvent inputEvent);

    bool JudgeCanMove(Enum.ENUM_InputEvent inputEvent);
}
