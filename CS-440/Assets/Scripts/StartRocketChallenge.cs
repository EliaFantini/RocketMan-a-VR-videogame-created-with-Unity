using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartRocketChallenge : MonoBehaviour
{
    
    //public SwitchControl switchControl;

    public ButtonLamp buttonLamp1;
    public ButtonLamp buttonLamp2;

    // Update is called once per frame
    public ButtonLamp finalLamp;
    void Update()
    {
        if(buttonLamp1.on == true && buttonLamp2.on) {
            //start rocket
            finalLamp.on = true;
            GameManager.Instance.UpdateGameState(RiddlesProgress.RocketLaunched);
        } else {
            //Do nothing
        }
    }
}

