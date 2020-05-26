using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BrokeEvent : MonoBehaviour
{
    public GameObject completeObj;
    public GameObject brokeObj;
    public GameObject platform;

    void Destroy()
    {
        GameObject.Destroy(transform.parent.gameObject);
    }

    public void Broken()
    {
        completeObj.SetActive(false);
        brokeObj.SetActive(true);
        CoroutineManager.StartCoroutineTask(() =>
        {
            transform.parent.DOScale(0f, 1f).SetEase(Ease.OutExpo).OnComplete(Destroy);
        }, 1f);
    }

}
