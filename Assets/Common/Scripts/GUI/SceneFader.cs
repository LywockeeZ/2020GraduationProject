using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : Singleton<SceneFader>
{
    public float DefaultDuration = 0.5f;

    private CanvasGroup m_canvasGroup;
    private float _fadeConter = 0;

    public bool StartFadeIn;
    public bool StartFadeOut;

    protected override void Awake()
    {
        base.Awake();
        m_canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }


    public void FadeIn()
    {
        StartFadeIn = true;
    }


    public void FadeOut()
    {
        StartFadeOut = false;
    }

    private void Update()
    {
        if (StartFadeIn)
        {
            m_canvasGroup.interactable = false;
            m_canvasGroup.blocksRaycasts = true;
            m_canvasGroup.alpha = Mathf.SmoothStep(m_canvasGroup.alpha, 1 ,  _fadeConter);
            _fadeConter += Time.deltaTime / DefaultDuration;

            if (m_canvasGroup.alpha == 1)
            {
                StartFadeIn = false;
                _fadeConter = 0;
            }

        }

        if (StartFadeOut)
        {
            m_canvasGroup.alpha = Mathf.SmoothStep(m_canvasGroup.alpha, 0, _fadeConter);
            _fadeConter += Time.deltaTime / DefaultDuration;

            if (m_canvasGroup.alpha == 0)
            {
                StartFadeOut = false;
                _fadeConter = 0;
                m_canvasGroup.interactable = true;
                m_canvasGroup.blocksRaycasts = false;
            }

        }
    }

}
