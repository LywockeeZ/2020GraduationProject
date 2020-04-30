using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChanger : Singleton<CameraChanger>
{
    private CinemachineVirtualCamera levelCamera;

    public void SetLevelCamera(CinemachineVirtualCamera cam)
    {
        levelCamera = cam;
    }

    public void ChangeCam()
    {
        if (levelCamera != null)
        {
            if (levelCamera.gameObject.activeInHierarchy == false)
            {
                levelCamera.gameObject.SetActive(true);
            }
            else
                levelCamera.gameObject.SetActive(false);
        }
    }
}
