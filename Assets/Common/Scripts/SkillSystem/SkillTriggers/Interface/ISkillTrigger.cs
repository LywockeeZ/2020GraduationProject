using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ISkillTrigger
{

    void Init();

    void Reset();

    ISkillTrigger Clone();

    bool Execute(ISkillCore instance);

    float GetStartTime();

    bool IsExecuted();

    string GetTypeName();
}
