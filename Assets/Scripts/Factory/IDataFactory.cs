using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDataFactory 
{
    public abstract StageMetaData LoadStageData(string StageName);
}
