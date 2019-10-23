using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpperUnit 
{
    BaseUnit CurrentOn { get; set; }
    float Height { get; set; }
    bool CanBeFire { get; set; }

    void Init();

    void End();

    Vector3 SetTargetPos(Vector3 _targetPos);

}
