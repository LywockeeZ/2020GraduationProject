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
    private bool hasPressed = false;

    void Start()
    {
        currentState = defaultState;
        //清空
        Game.Instance.SetLevelWillToOnMain(null, null);
    }

    void Update()
    {
        switch (currentState)
        {
            case StartSceneState.Entering:
                if (Input.GetKeyDown(KeyCode.Space) && !hasPressed)
                {
                    LoadingSceneManager.LoadScene("StartSceneBack");
                    hasPressed = true;
                }
                break;
            case StartSceneState.Entered:
                if (Input.anyKeyDown && !hasPressed)
                {
                    LoadingSceneManager.LoadScene("Part1");
                    Game.Instance.ClearUnlockedSkills();
                    hasPressed = true;
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
