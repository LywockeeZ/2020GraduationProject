using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDataFactory 
{
    public abstract Dictionary<string, IStageHandler> LoadStageData();
}
