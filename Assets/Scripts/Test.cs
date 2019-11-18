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
        //XmlTool.XmlCreate("StageData");
        //Debug.Log(XmlTool.LoadStageDataXml("StageData", "第1关"));
        Game.Instance.Initinal();
        Game.Instance.RegisterGameEvent(ENUM_GameEvent.NewStage, new NewStageObserver());
    }

    void Update()
    {
        Game.Instance.Updata();
    }

    public void OnClick()
    {
        Game.Instance.LoadNextStage();
        Game.Instance.NotifyGameEvent(ENUM_GameEvent.NewStage, null);
    }

}
