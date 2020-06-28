using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using DG.Tweening;

public class SkillIndicator : MonoBehaviour
{
    public SpriteRenderer indicator;
    public SpriteRenderer emittor;
    public bool haveIndicator = true;

    private Tweener indicatorTweener;
    private Tweener emittorTweener;
    private bool isMouseIn = false;
    private bool isHighlight = false;
    private bool isCancelAll = false;

    private void Start()
    {
        indicatorTweener = indicator.DOFade(1, 0.1f).From(0).SetAutoKill(false);
        emittorTweener = emittor.DOFade(1, 0.1f).From(0).SetAutoKill(false);
        indicatorTweener.Pause();
        emittorTweener.Pause();

    }

    private void Update()
    {
        if (isMouseIn)
        {
            if (!isHighlight)
            {
                Highlight();
                isHighlight = true;
            }
            isMouseIn = false;
        }
        else
        if (isHighlight)
        {
            HighlightCancel();
        }
    }

    public void ShowIndicator()
    {
        if (haveIndicator)
        {
            indicatorTweener.PlayForward();
            emittorTweener.PlayBackwards();
        }
    }

    public void HideIndicator()
    {
        indicatorTweener.PlayBackwards();
        emittorTweener.PlayBackwards();
    }

    public void ShowEmitter()
    {
        //emittorTweener.PlayForward();
        if (haveIndicator)
        {
            indicatorTweener.PlayForward();
            transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("CanEmmit");
        }
    }

    public void HideEmitter()
    {
        if (isHighlight)
        {
            HighlightCancel();            
        }
        transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
        emittorTweener.PlayBackwards();
        indicatorTweener.PlayBackwards();
    }

    public void Highlight()
    {
        indicatorTweener.PlayBackwards();
        emittorTweener.PlayForward();
        transform.parent.GetChild(0).GetComponent<Highlighter>().ConstantOn(Color.white, 0.1f);
    }

    public void HighlightCancel()
    {
        emittorTweener.PlayBackwards();
        indicatorTweener.PlayForward();
        transform.parent.GetChild(0).GetComponent<Highlighter>().ConstantOff(0.1f);
        isHighlight = false;
        isMouseIn = false;
    }

    public void SetIsMouseIn(bool value)
    {
        isMouseIn = value;
    }

    private void OnDisable()
    {
        transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
        if (isHighlight)
        {
            HighlightCancel();
        }
        emittorTweener.Rewind();
        indicatorTweener.Rewind();

    }
}
