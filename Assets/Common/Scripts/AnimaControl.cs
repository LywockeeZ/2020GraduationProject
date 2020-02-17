using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimaControl : MonoBehaviour
{
    public DOTweenAnimation anima;
    bool isFirst = true;
    bool isHappened = false;
    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {
        if (isFirst)
        {
            anima.DOPlayNext();
            isFirst = false;
        }
    }

}
