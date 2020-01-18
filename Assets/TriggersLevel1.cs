using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggersLevel1 : MonoBehaviour
{
    public void Trigger1()
    {
        Game.Instance.SetCanInput(false);
        Game.Instance.SetCanFreeMove(false);
    }
}
