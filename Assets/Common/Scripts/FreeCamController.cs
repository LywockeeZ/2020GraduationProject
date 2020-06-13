using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeCamController : Singleton<FreeCamController>
{
    public CinemachineTargetGroup targetGroup;
    public Transform playerTrans;
    public CinemachineFreeLook freeLookCam;

    private Vector3 pos;
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        freeLookCam.transform.position = playerTrans.position - playerTrans.forward * 3 + Vector3.up * 2;
        freeLookCam.enabled = true;
        OnLive();
    }

    private void OnDisable()
    {
        freeLookCam.enabled = false;
        freeLookCam.Follow = null;
        freeLookCam.LookAt = null;
    }

    void Start()
    {
        freeLookCam.m_XAxis.m_InputAxisName = null;
        freeLookCam.m_YAxis.m_InputAxisName = null;
    }

    void Update()
    {

        if (Input.GetMouseButton(1) && Game.Instance.GetCanInput())
        {
            freeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
            freeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";
        }
        else
        {
            if (freeLookCam != null)
            {
                freeLookCam.m_YAxis.m_InputAxisName = null;
                freeLookCam.m_XAxis.m_InputAxisName = null;
                freeLookCam.m_XAxis.m_InputAxisValue = 0;
                freeLookCam.m_YAxis.m_InputAxisValue = 0;
            }
        }

    }

    public void OnLive()
    {
        freeLookCam.Follow = playerTrans;
        freeLookCam.LookAt = playerTrans;
        freeLookCam.m_XAxis.Value = 0;
        freeLookCam.m_YAxis.Value = 0.5f;

    }

    public void AddTarget(Transform target, float weight, float radius)
    {
        targetGroup.AddMember(target, weight, radius);
    }

    public void RemoveTarget(Transform target)
    {
        targetGroup.RemoveMember(target);
    }
}
