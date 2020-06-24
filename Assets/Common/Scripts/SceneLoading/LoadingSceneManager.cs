using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// 加载场景的类，使用一个Loading界面而不是默认的API
/// </summary>
public class LoadingSceneManager : MonoBehaviour
{
    public static string LoadingScreenSceneName = "LoadingScreen";

    protected AsyncOperation _asyncOperation;
    protected static string _sceneToLoad = "";      //保存目标场景名
    protected static string _loadSceneName = null;
    private static bool isLoadingSceneEnd = false;

    /// <summary>
    /// 一旦切换到加载界面的场景就立刻开始异步加载
    /// </summary>
    protected virtual void Start()
    {
        if (_sceneToLoad != "")
        {
            StartCoroutine(LoadAsynchronously());
        }

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }


    /// <summary>
    /// 通过这个静态方法可以从任何地方加载一个目标场景
    /// </summary>
    /// <param name="sceneToLoad"></param>
    public static void LoadScene(string sceneToLoad)
    {
        _sceneToLoad = sceneToLoad;
        Application.backgroundLoadingPriority = ThreadPriority.High;
        if (LoadingScreenSceneName != null)
        {
            //先加载到加载界面
            SceneChanger.Instance.LoadScene(LoadingScreenSceneName);
            isLoadingSceneEnd = true;
        }
    }


    /// <summary>
    /// 当有不同的加载界面时，通过这个方法调用另外的加载界面
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <param name="loadingSceneName"></param>
    public static void LoadScene(string sceneToLoad, string loadingSceneName)
    {
        _sceneToLoad = sceneToLoad;
        _loadSceneName = loadingSceneName;
        Application.backgroundLoadingPriority = ThreadPriority.High;
        SceneChanger.Instance.LoadScene(loadingSceneName);
        //SceneManager.LoadScene(loadingSceneName);
        isLoadingSceneEnd = false;
    }


    protected virtual IEnumerator LoadAsynchronously()
    {
        _asyncOperation = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Single);
        _asyncOperation.allowSceneActivation = false;

        //待外界调用StartLoad,开始加载后再进行场景加载，用于过场场景
        while(!isLoadingSceneEnd)
        {
            yield return null;
        }
        //待加入进度条相关的操作
        yield return new WaitForSeconds(0.2f);

        SceneChanger.Instance.LoadSceneComplete();

        //切换到新的场景
        _asyncOperation.allowSceneActivation = true;

        Func<bool> call;
        CoroutineManager.StartCoroutineTask(call = () => { return _asyncOperation.isDone; }, ENUM_GameEvent.LoadSceneComplete, 0f);

    }

    public virtual void StartLoad()
    {
        isLoadingSceneEnd = true;
    }

}
