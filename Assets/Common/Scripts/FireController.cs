using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FireController:MonoBehaviour
{
    public GameObject fire; 

    private bool isFirstOpen = true;
    private bool isEnd = false;

    private void OnEnable()
    {
        if (!isFirstOpen)
        {
            FireAppear();
        }
    }

    void Start()
    {
        FireAppear();
        isFirstOpen = false;
    }


    void Update()
    {
        
    }

    public void FireAppear()
    {
        float n = Random.Range(0.8f, 1.4f);
        fire.transform.DOScale(new Vector3(n, n, n), 0.5f).ChangeStartValue(Vector3.zero);
        isEnd = false;
    }

    public void FireDisappear()
    {
        fire.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad).OnComplete(()=> { isEnd = true; });
    }

    public bool IsFireEnd()
    {
        return isEnd;
    }
}
