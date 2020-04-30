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
        freeLookCam = GetComponent<CinemachineFreeLook>();
        pos = transform.position;
    }

    private void OnEnable()
    {
        OnLive();
    }

    private void OnDisable()
    {
        freeLookCam.Follow = null;
        freeLookCam.LookAt = null;
        transform.position = pos;
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
