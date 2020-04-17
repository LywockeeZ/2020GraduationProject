using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeCamController : Singleton<FreeCamController>
{
    public CinemachineTargetGroup targetGroup;

    private CinemachineFreeLook freeLookCam;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        freeLookCam = GetComponent<CinemachineFreeLook>();
        freeLookCam.m_XAxis.m_InputAxisName = null;
        freeLookCam.m_YAxis.m_InputAxisName = null;
    }

    void Update()
    {

        if (Input.GetMouseButton(1))
        {
            freeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
            freeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";
        }
        else
        {
            freeLookCam.m_YAxis.m_InputAxisName = null;
            freeLookCam.m_XAxis.m_InputAxisName = null;
            freeLookCam.m_XAxis.m_InputAxisValue = 0;
            freeLookCam.m_YAxis.m_InputAxisValue = 0;
        }

    }

    public void OnLive()
    {
        freeLookCam.Follow = Game.Instance.GetPlayerUnit().transform;
        AddTarget(Game.Instance.GetPlayerUnit().transform, 4, 0);
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
