using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMLDataFactory : IDataFactory
{
    public override StageMetaData LoadStageData(string StageName)
    {
        return XmlTool.LoadStageDataXml("StageData", StageName);
    }
}
