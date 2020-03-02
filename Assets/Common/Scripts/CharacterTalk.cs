using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class CharacterTalk : MonoBehaviour
{
    public GameObject trigger1;

    private void Start()
    {
        trigger1.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        Game.Instance.SetCanInput(false);
        Game.Instance.SetCanFreeMove(false);
        Flowchart.BroadcastFungusMessage("Character1");
        trigger1.SetActive(true);
    }

    public void OnTalkEnd()
    {
        Game.Instance.SetCanFreeMove(true);
        Game.Instance.SetCanInput(true);
    }
}
