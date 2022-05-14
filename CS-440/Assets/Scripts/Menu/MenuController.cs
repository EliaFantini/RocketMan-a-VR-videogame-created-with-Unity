using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public FadeScreen fadeScreen;

    private void Start()
    {
        OVRManager.display.RecenterPose();
    }
    public void PlayButton()
    {
        fadeScreen.fadeOut(changeScene, "Experiment2");
        
    }

    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
