using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraChanger : Singleton<CameraChanger>
{
    public bool haveStartCam;
    public CinemachineVirtualCamera startCam;
    private CinemachineVirtualCamera levelCamera;
    private bool hasChange = false;

    private void Start()
    {
        if (haveStartCam)
        {
            Game.Instance.SetCanInput(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && haveStartCam && LevelManager.Instance.LevelName == SceneManager.GetActiveScene().name && !hasChange)
        {
            startCam.gameObject.SetActive(false);
            hasChange = true;
            void action() { Game.Instance.SetCanInput(true); };
            CoroutineManager.StartCoroutineTask(action, 0.2f);
        }
    }
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
