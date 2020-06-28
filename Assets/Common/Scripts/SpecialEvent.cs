using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class SpecialEvent : Singleton<SpecialEvent>
{
    public bool DiedByTouchBee = false;
    public int DiedByRescueFail = 0;

    public void CheckEvent()
    {
        if (DiedByTouchBee)
        {
            Flowchart.BroadcastFungusMessage("touchBee");
            DiedByTouchBee = false;
        }
        else
        if (DiedByRescueFail == 1)
        {
            Flowchart.BroadcastFungusMessage("rescueFail");
        }
    }
}
