using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonTag : MonoBehaviour
{
    public Sprite tagToShow;
    public float moveDistence = 0;
    public GameObject tag;

    private Image image;
    private RectTransform rect;
    private Tweener tweener1;
    private Tweener tweener2;
    void Start()
    {
        image = tag.GetComponent<Image>();
        rect = tag.GetComponent<RectTransform>();
        tweener1 = rect.DOLocalMoveY(rect.localPosition.y - moveDistence, 0.2f).SetAutoKill(false);
        tweener2 = image.DOFade(1, 0.2f).From(0).SetAutoKill(false);
        tweener1.Pause();
        tweener2.Pause();
    }

    public void ShowTag()
    {
        tweener1.PlayForward();
        tweener2.PlayForward();
    }

    public void HideTag()
    {
        tweener1.PlayBackwards();
        tweener2.PlayBackwards();
    }

    public void OnDisable()
    {
        tweener1.Rewind();
        tweener2.Rewind();
    }
}
