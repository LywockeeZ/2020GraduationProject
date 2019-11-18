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
        Game.Instance.Initinal();
    }

    void Update()
    {
        Game.Instance.Updata();
    }

    public void StartStage()
    {
        Game.Instance.NotifyEvent(ENUM_GameEvent.StageBegain, null);
    }


    public void StartRound()
    {
        Game.Instance.NotifyEvent(ENUM_GameEvent.RoundBegain, null);
    }


}
