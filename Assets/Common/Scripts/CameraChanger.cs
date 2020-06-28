using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraChanger : Singleton<CameraChanger>
{
    public bool haveStartCam;
    public bool autoSetCanInput = true;
    public CinemachineVirtualCamera startCam;
    private CinemachineVirtualCamera levelCamera;
    private bool hasChange = false;

    private void Start()
    {
        if (haveStartCam)
        {
            void action() { Game.Instance.SetCanInput(false); };
            CoroutineManager.StartCoroutineTask(action, 0.1f);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && haveStartCam && LevelManager.Instance.LevelName == SceneManager.GetActiveScene().name && !hasChange && autoSetCanInput)
        {
            startCam.gameObject.SetActive(false);
            hasChange = true;
            void action() { Game.Instance.SetCanInput(true); };
            CoroutineManager.StartCoroutineTask(action, 0.5f);
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

    public void ChangeStartCam()
    {
        startCam.gameObject.SetActive(false);
    }
}
