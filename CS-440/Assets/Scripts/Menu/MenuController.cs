using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for the menu, it changes the scene when needed
/// </summary>
public class MenuController : MonoBehaviour
{
    public FadeScreen fadeScreen;

    /// <summary>
    /// Fade out of the menu and change scene on pressing the play button
    /// </summary>
    public void PlayButton()
    {
        fadeScreen.fadeOut(changeScene, "Rocket");
        
    }

    /// <summary>
    /// Method to load a scene
    /// </summary>
    /// <param name="sceneName">The scene name</param>
    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
