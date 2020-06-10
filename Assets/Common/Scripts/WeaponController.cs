using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    Kinfe
}

public class WeaponController : Singleton<WeaponController>
{
    public delegate void mydelegate();
    public event mydelegate setWaterSac;
    public event mydelegate throwWaterSac;
    public event mydelegate setWaterBag;
    public event mydelegate throwWaterBag;

    public List<GameObject> weapons;
    public Transform rightHandPos;
    public Transform leftHandPos;
    public Transform hip;
    public Transform firePos;

    private Animator animator;
    private GameObject currentWeapon;
    private Vector3 originPos;
    private Quaternion originRotation;
    private Transform originParent;
    private bool isFirstTime = true;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        originPos = weapons[0].transform.localPosition;
        originRotation = weapons[0].transform.localRotation;
        originParent = hip;
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = Vector3.zero;
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
        CoroutineManager.StartCoroutineTask(action, 0f);
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

    public void EndPump()
    {
        animator.SetTrigger("item_PumpEnd");
    }


    /// <summary>
    /// 由动画调用
    /// </summary>
    public void SetKnife()
    {
        originPos = weapons[0].transform.localPosition;
        originRotation = weapons[0].transform.localRotation;
        originParent = hip;
        currentWeapon = weapons[0];
        currentWeapon.transform.SetParent(rightHandPos);
        currentWeapon.transform.localPosition = new Vector3(-0.00122f, 0.0007f, 0f);
        currentWeapon.transform.localRotation = Quaternion.Euler(new Vector3(-18.948f, 30.631f, -109.925f));
    }

    public void CancelKnife()
    {
        if (!isFirstTime)
        {
            if (currentWeapon != null)
            {
                currentWeapon.transform.SetParent(hip);
                currentWeapon.transform.localPosition = new Vector3(-0.00287f, 0.00287f, 0.00439f);
                currentWeapon.transform.localRotation = Quaternion.Euler(new Vector3(-2.589f, 53.194f, -0.568f));
                currentWeapon = null;
            }
        }
        else isFirstTime = false;
    }

    public void SetPump()
    {
        originPos = weapons[1].transform.localPosition;
        originRotation = weapons[1].transform.localRotation;
        originParent = hip;
        currentWeapon = weapons[1];
        currentWeapon.transform.SetParent(leftHandPos);
        currentWeapon.transform.localPosition = new Vector3(0.0039f, 0.0017f, 0.001f);
        currentWeapon.transform.localRotation = Quaternion.Euler(new Vector3(64.979f, 72.316f, 11.165f));
    }

    public void CancelPump()
    {
        currentWeapon.transform.SetParent(originParent);
        currentWeapon.transform.localPosition = originPos;
        currentWeapon.transform.localRotation = originRotation;
        currentWeapon = weapons[0];
    }

    public void SetWaterSac()
    {
        setWaterSac.Invoke();
    }

    public void ThrowWaterSac()
    {
        throwWaterSac.Invoke();
    }

    public void SetWaterBag()
    {
        setWaterBag.Invoke();
    }

    public void ThrowWaterBag()
    {
        throwWaterBag.Invoke();
    }

    public void ResetImediately()
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.SetParent(originParent);
            currentWeapon.transform.localPosition = originPos;
            currentWeapon.transform.localRotation = originRotation;
        }
    }
}
