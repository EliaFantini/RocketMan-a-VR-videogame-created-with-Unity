using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiresRiddleController : MonoBehaviour
{
    [SerializeField]
    public GameObject screen;
    public ButtonTrap button;
    public GameObject[] outletArr;
    public GameObject[] correctPlugArr;
    public AttachWirePlug[] socketArr;
    public int correctlyPluggedCounter;

    void Start()
    {
        correctlyPluggedCounter = 0;
        outletArr = new GameObject[5];
    }

    public void turnOnScreen()
    {
        screen.SetActive(true);
    }

    public void addCorrectlyPlugged()
    {
        correctlyPluggedCounter++;
        if (correctlyPluggedCounter >= 5)
        {
            button.enableButton();
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
