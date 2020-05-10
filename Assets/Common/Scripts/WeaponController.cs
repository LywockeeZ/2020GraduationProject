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
    public Transform headPos;

    private Animator animator;
    private GameObject currentWeapon;
    private Vector3 originPos;
    private Quaternion originRotation;
    private Transform originParent;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void OnDisable()
    {
        DetachEvent();
    }

    private EventListenerDelegate OnStageBegain;
    private EventListenerDelegate OnLoadSceneStart;
    private void RegisterEvent()
    {
        Game.Instance.RegisterEvent(ENUM_GameEvent.StageBegain,
            OnStageBegain = (Message evt) =>
            {
                StartKnife();
            });
    }

    private void DetachEvent()
    {
        Game.Instance.DetachEvent(ENUM_GameEvent.StageBegain, OnStageBegain);
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
        currentWeapon.transform.SetParent(headPos);
        currentWeapon.transform.localPosition = new Vector3(-0.055f, 0.045f, -0.006f);
        currentWeapon.transform.localRotation = Quaternion.Euler(new Vector3(-12.75f, 46.5f, -141.14f));
    }

    public void CancelKnife()
    {
        currentWeapon.transform.SetParent(originParent);
        currentWeapon.transform.localPosition = originPos;
        currentWeapon.transform.localRotation = originRotation;
    }
}
