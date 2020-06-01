using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StartSceneState
{
    Entering,
    Entered
}

public class StartSceneUI : MonoBehaviour
{
    public StartSceneState defaultState;
    private StartSceneState currentState;

    void Start()
    {
        currentState = defaultState;
    }

    void Update()
    {
        switch (currentState)
        {
            case StartSceneState.Entering:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    LoadingSceneManager.LoadScene("StartSceneBack");
                }
                break;
            case StartSceneState.Entered:
                if (Input.anyKeyDown)
                {
                    LoadingSceneManager.LoadScene("Part1");
                    Game.Instance.ClearUnlockedSkills();
                }
                break;
            default:
                break;
        }
    }

    public void ChangeToEntered()
    {
        currentState = StartSceneState.Entered;
    }

    public void ChangeToEntering()
    {
        currentState = StartSceneState.Entering;
    }
}
