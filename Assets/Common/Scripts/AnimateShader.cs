using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateShader : MonoBehaviour
{
    public float interval = 0.1f;
    public List<GameObject> projects;

    public GameObject currentProject;
    private WaitForSeconds waitTime;
    private int index = 0;
    private int count;
    private Coroutine coroutine;
    private bool isStart = false;
    void Start()
    {
        currentProject = projects[0];
        waitTime = new WaitForSeconds(interval);
        count = projects.Count;
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
        isStart = false;
    }

    void Update()
    {
        if (!isStart)
        {
            coroutine = StartCoroutine(Animated());
            isStart = true;
        }
    }

    IEnumerator Animated()
    {
        while(true)
        {
            Debug.Log(index);
            currentProject.SetActive(false);
            currentProject = projects[index];
            currentProject.SetActive(true);
            index = (index + 1) % count;
            yield return waitTime;
            yield return null;
        }
    }

}
