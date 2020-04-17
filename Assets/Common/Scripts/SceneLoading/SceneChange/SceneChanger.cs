using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{ 

    public Animator animator;
    private int sceneToLoad;
    private string sceneToLoadstr;
    private bool isLoadingScene = false;
    
    private static GameObject MonoSingletionRoot;
    private static SceneChanger instance;
    public static SceneChanger Instance
    {
        get
        {
            if (MonoSingletionRoot==null)
            {
                MonoSingletionRoot = GameObject.Find("SceneChanger");
                if (MonoSingletionRoot==null)
                {
                    MonoSingletionRoot = new GameObject();
                    MonoSingletionRoot.name = "SceneChanger";
                    DontDestroyOnLoad(MonoSingletionRoot);
                }
            }

            if (instance == null)
            {
                instance = MonoSingletionRoot.GetComponent<SceneChanger>();
                if (instance == null)
                {
                    instance = MonoSingletionRoot.AddComponent<SceneChanger>();
                }
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
    }

    public void FadeToNextScene()
    {
        FadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //触发fade动画
    public void FadeToScene(int sceneIndex)
    {
        sceneToLoad = sceneIndex;
        animator.SetTrigger("FadeOut");
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
                SceneManager.LoadScene("LoadingScreen");
            }
            CoroutineManager.StartCoroutineTask(action, 0f);
        }
    }

    /// <summary>
    /// 由LoadsceneManager调用，在加载场景之前先由此触发淡出
    /// </summary>
    public void LoadScene(string sceneName)
    {
        isLoadingScene = true;
        sceneToLoadstr = sceneName;
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
    public void FadeScene()
    {
        animator.SetTrigger("FadeOutAndIn");
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }
}
