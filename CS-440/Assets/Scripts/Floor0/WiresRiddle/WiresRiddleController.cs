using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller of the wires riddle
/// </summary>
public class WiresRiddleController : MonoBehaviour
{
    [SerializeField]
    public GameObject[] screens;
    public ButtonTrap button;
    public GameObject[] outletArr;
    public GameObject[] correctPlugArr;
    public AttachWirePlug[] socketArr;
    public int correctlyPluggedCounter;
    public AudioClip wrongSound;

    void Start()
    {
        correctlyPluggedCounter = 0;
        outletArr = new GameObject[5];
        // Scren is turned off
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }
    }

    /// <summary>
    /// Turn on the screen with the solution of the wires placement
    /// </summary>
    public void turnOnScreen()
    {
        foreach(GameObject screen in screens)
        {
            screen.SetActive(true);
            GameManager.Instance.UpdateGameState(RiddlesProgress.PowerPlugged);
        }
    }

    /// <summary>
    /// Increment a counter of correctly plugged wires
    /// If all wires are plugged in, enable the button of the trap door
    /// </summary>
    public void addCorrectlyPlugged()
    {
        correctlyPluggedCounter++;
        if (correctlyPluggedCounter >= 5)
        {
            button.enableButton();
            GameManager.Instance.UpdateGameState(RiddlesProgress.WiresPattern);
        }

    }
    public void removeCorrectlyPlugged()
    {
        correctlyPluggedCounter--;

    }

    /// <summary>
    /// Called when a wire is plugged in, check if correctly plugged
    /// If all wires are plugged but at least one is wrong, play an "wrong" sound
    /// </summary>
    /// <param name="plug"> The wire plugged in</param>
    /// <param name="outletId">The ID of the plug </param>
    public void plugged(GameObject plug, int outletId)
    {
        if (correctPlugArr[outletId] == plug)
        {
            addCorrectlyPlugged();
        }
        // Add new wires plug
        outletArr[outletId] = plug;
        int i=0;
        //Check if all wires are plugged in
        while (true)
        {
            if (outletArr[i] == null)
            {
                return;
            }
            if (i == 4)
            {
                break;
            }
            i++;        
        }
        i = 0;
        //If all wires are plugged in, check if they are all correct
        while (true)
        {
            if (outletArr[i] != correctPlugArr[i])
            {
                //Play sound if one is uncorrect
                AudioSource.PlayClipAtPoint(wrongSound, plug.transform.position);
                return;
            }
            if (i == 4)
            {
                break;
            }
            i++;
        }
    }

    /// <summary>
    /// Unplug all wires, except if they are correctly plugged in
    /// </summary>
    public void unplugAll()
    {
        if (correctlyPluggedCounter >= 5)
        {
            return;
        }
        else
        {
            foreach (AttachWirePlug socket in socketArr)
            {
                socket.detach();
            }
            outletArr = new GameObject[5];
            correctlyPluggedCounter = 0;
        }
            
    }
}
