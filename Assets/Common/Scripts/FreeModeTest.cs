using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeModeTest : MonoBehaviour
{
    private void Awake()
    {
        Game.Instance.Initinal();
        Game.Instance.SetCanFreeMove(true);
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        Game.Instance.Updata();
    }
}
