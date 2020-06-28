using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAnimatorPara : MonoBehaviour
{
    private string animatorPara = "whirlwind";
    private Animator animator;
    private bool isFirstTime = true;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        if (!isFirstTime)
        {
            animator.SetTrigger(animatorPara);
        }
        else isFirstTime = false;
    }

    public void SetAnimatorBool(string name)
    {
        animatorPara = name;
    }

    public void TriggerAnimator()
    {
        animator.SetTrigger(animatorPara);
    }
}
