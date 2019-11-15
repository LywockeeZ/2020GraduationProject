using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{


    private void Awake()
    {
    }
    void Start()
    {
        XmlTool.XmlCreate("StageData");
        Debug.Log(XmlTool.LoadStageDataXml("StageData", "第1关"));
    }

    void Update()
    {
        
    }

}
