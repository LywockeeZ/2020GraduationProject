using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    Kinfe
}

public class WeaponController : Singleton<WeaponController>
{
    public List<GameObject> weapons;
    public Transform handPos;

    private Animator animator;
    private GameObject currentWeapon;
    private Vector3 originPos;
    private Quaternion originRotation;
    private Transform originParent;
    private bool isFirstTime = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void OnEnable()
    {
        void action()
        {
            if (Game.Instance.GetIsInStage())
            {
                StartKnife();
            }
            else
            {
                EndKnife();
            }
        }
        CoroutineManager.StartCoroutineTask(action, 0.2f);
        RegisterEvent();
    }

    private void OnDisable()
    {
        DetachEvent();
    }

    private EventListenerDelegate OnStageBegain;
    private EventListenerDelegate OnStageEnd;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageBegain,
            OnStageBegain = (Message evt) =>
            {
                StartKnife();
            });
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageEnd,
            OnStageEnd = (Message evt) =>
            {
                EndKnife();
            });

    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.StageBegain, OnStageBegain);
        Game.Instance.DetachEvent(ENUM_GameEvent.StageEnd, OnStageEnd);
    }


    public void StartKnife()
    {
        animator.SetTrigger("SetKnife");
    }

    public void EndKnife()
    {
        animator.SetTrigger("CancelKnife");
    }


    /// <summary>
    /// 由动画调用
    /// </summary>
    public void SetKnife()
    {
        currentWeapon = weapons[0];
        originPos = currentWeapon.transform.localPosition;
        originRotation = currentWeapon.transform.localRotation;
        originParent = currentWeapon.transform.parent;
        currentWeapon.transform.SetParent(handPos);
        currentWeapon.transform.localPosition = new Vector3(-0.00122f, 0.0007f, 0f);
        currentWeapon.transform.localRotation = Quaternion.Euler(new Vector3(-18.948f, 30.631f, -109.925f));
    }

    public void CancelKnife()
    {
        if (!isFirstTime)
        {
            currentWeapon.transform.SetParent(originParent);
            currentWeapon.transform.localPosition = originPos;
            currentWeapon.transform.localRotation = originRotation;
        }
        else isFirstTime = false;
    }
}
