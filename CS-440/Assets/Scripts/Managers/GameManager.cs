using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public RiddlesProgress currentState;
    private bool[] riddlesSolved;
    public FadeScreen fadeScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            riddlesSolved = new bool[RiddlesProgress.GetValues(typeof(RiddlesProgress)).Length];
            DontDestroyOnLoad(gameObject);
            UpdateGameState(RiddlesProgress.Start);
        }
        else
        {
            Destroy(gameObject);
        }       
    }

    public void UpdateGameState(RiddlesProgress riddle)
    {
        currentState = riddle;
        int stateIndex = (int)riddle;
        riddlesSolved[stateIndex] = true;
        switch (riddle)
        {
            case RiddlesProgress.PowerPlugged:
                break;
            case RiddlesProgress.WiresPattern:
                break;
            case RiddlesProgress.TrapDoorButton:
                break;
            case RiddlesProgress.UVLightGrabbed:
                break;
            case RiddlesProgress.OpenBoxSign:
                break;
            case RiddlesProgress.BrakeWindow:
                break;
            case RiddlesProgress.TurnOnEngine:
                break;
            case RiddlesProgress.ThreeDigitsCode:
                break;
            case RiddlesProgress.ThreeButtonsRiddle:
                break;
            case RiddlesProgress.RocketLaunched:
                break;
            case RiddlesProgress.FireEstinguished:
                break;
            case RiddlesProgress.FrameAttached:
                break;
            case RiddlesProgress.UnderTableButton:
                break;
            case RiddlesProgress.DoorCodeInserted:
                break;
            default:
                break;

        }
        checkForSceneChange();
    }

    private void checkForSceneChange()
    {
        if( riddlesSolved[(int)RiddlesProgress.RocketLaunched])
        {
            StartCoroutine(RocketLaunch());
        }
    }


    public IEnumerator RocketLaunch()
    {
        AnimationManager.Instance.rocketLaunchAnimation();
        yield return new WaitForSeconds(5);
        fadeScreen.fadeOut(changeScene, "RocketInSpace");

    }

    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}

public enum RiddlesProgress{
    Start,
    PowerPlugged,
    WiresPattern,
    TrapDoorButton,
    UVLightGrabbed,
    OpenBoxSign,
    ScrewsRemoved,
    BrakeWindow,
    TurnOnEngine,
    ThreeDigitsCode,
    ThreeButtonsRiddle,
    RocketLaunched,
    FireEstinguished,
    FrameAttached,
    UnderTableButton,
    DoorCodeInserted
}
