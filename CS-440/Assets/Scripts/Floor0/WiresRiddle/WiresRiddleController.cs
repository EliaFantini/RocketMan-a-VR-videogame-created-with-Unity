using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }
    }

    public void turnOnScreen()
    {
        foreach(GameObject screen in screens)
        {
            screen.SetActive(true);
            GameManager.Instance.UpdateGameState(RiddlesProgress.PowerPlugged);
        }
    }

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
    public void plugged(GameObject plug, int outletId)
    {
        if (correctPlugArr[outletId] == plug)
        {
            addCorrectlyPlugged();
        }
        outletArr[outletId] = plug;
        int i=0;
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
        while (true)
        {
            if (outletArr[i] != correctPlugArr[i])
            {
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
