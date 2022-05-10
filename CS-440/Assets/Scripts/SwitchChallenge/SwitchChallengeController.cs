using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchChallengeController : MonoBehaviour
{
    
    //public SwitchControl switchControl;

    public ButtonLamp buttonLamp1;
    public ButtonLamp buttonLamp2;
    public ButtonLamp buttonLamp3;
    public ButtonLamp buttonLamp4;

    public ButtonLamp finalLamp;

    // Update is called once per frame
    void Update()
    {
        if(buttonLamp1.on == true && buttonLamp2.on == true &&
        buttonLamp3.on == true && buttonLamp4.on == true) {
            finalLamp.on = true;
        } else {
            finalLamp.on =false;
        }
    }
}
