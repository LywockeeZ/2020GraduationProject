using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SceneChanger : Singleton<SceneChanger>
{ 

    public Animator animator;

    private int sceneToLoad;
    private string sceneToLoadstr = "LoadingScreen";
    private bool isLoadingScene = false;
    private Action callBack;
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {

    }


    public void FadeToNextScene()
    {
        FadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //触发fade动画
    public void FadeToScene(int sceneIndex)
    {
        sceneToLoad = sceneIndex;
        animator.SetTrigger("scene_FadeOut");
    }

    //加载
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    //由动画事件调用
    public void OnFadeCompleteStr()
    {
        //加载到加载界面场景
        if (isLoadingScene)
        {
            Game.Instance.NotifyEvent(ENUM_GameEvent.LoadSceneStart);
            void action()
            {
                SceneManager.LoadScene(sceneToLoadstr);
            }
            CoroutineManager.StartCoroutineTask(action, 0f);
        }
        else
        {
            if (callBack != null)
            {
                callBack();
            }
            CoroutineManager.StartCoroutineTask(FadeIn, 0.5f);
        }
    }

    /// <summary>
    /// 由LoadsceneManager调用，在加载场景之前先由此触发淡出
    /// </summary>
    public void LoadScene(string cutSceneName)
    {
        isLoadingScene = true;
        sceneToLoadstr = cutSceneName;
        FadeOut();
    }

    /// <summary>
    /// 由LoadsceneManager调用，在加载场景之后由此触发淡入
    /// </summary>
    public void LoadSceneComplete()
    {
        isLoadingScene = false;
        FadeIn();
    }

    /// <summary>
    /// 用来渐变过渡
    /// </summary>
    public void FadeScene(Action action)
    {
        callBack = action;
        FadeOut();
    }

    public void FadeOut()
    {
        GetComponent<AudioSource>().Play();
        animator.SetTrigger("scene_FadeOut");
    }

    public void FadeIn()
    {
        GetComponent<AudioSource>().Play();
        animator.SetTrigger("scene_FadeIn");
    }
}
