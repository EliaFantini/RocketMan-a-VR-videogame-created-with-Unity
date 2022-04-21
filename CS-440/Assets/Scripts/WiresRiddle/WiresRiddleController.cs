using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiresRiddleController : MonoBehaviour
{
    [SerializeField]
    public GameObject screen;
    public ButtonTrap button;
    bool powerPlugged;
    public int correctlyPluggedCounter;
    // Start is called before the first frame update
    void Start()
    {
        powerPlugged = false;
        correctlyPluggedCounter = 0;
    }

    public void turnOnScreen()
    {
        screen.SetActive(true);
    }

    public void turnOffScreen()
    {
        screen.SetActive(false);
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

}
