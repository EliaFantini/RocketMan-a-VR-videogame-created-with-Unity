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
    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            riddlesSolved = new bool[(int)RiddlesProgress.DoorCodeInserted + 1];
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
        
        int stateIndex = (int)riddle;
        riddlesSolved[stateIndex] = true;
        if ((int)riddle > (int)currentState)
        {
            currentState = riddle;
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
        SoundManager.Instance.playCountdownClip();
        yield return new WaitForSeconds(7);
        SoundManager.Instance.playRocketClip();

        AnimationManager.Instance.rocketLaunchAnimation();
        yield return new WaitForSeconds(7);
        fadeScreen.fadeOut(changeScene, "RocketInSpace");

    }

    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void endGame()
    {
        if (riddlesSolved[(int)RiddlesProgress.DoorCodeInserted] && !gameEnded)
        {
            gameEnded = true;
            StartCoroutine(capsuleLaunch());
        }

       
    }

    public IEnumerator capsuleLaunch()
    {
        AnimationManager.Instance.moveCapsuleWall();
        SoundManager.Instance.playCapsuleClip();
        yield return new WaitForSeconds(15);
        AnimationManager.Instance.capsuleLaunchAnimation();
        yield return new WaitForSeconds(5);
        fadeScreen.fadeOut(changeScene, "MainMenu");

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
    SwitchesRiddle,
    RocketLaunched,
    FireEstinguished,
    FrameFallen,
    FrameAttached,
    ExitPushed,
    DoorCodeInserted
}
