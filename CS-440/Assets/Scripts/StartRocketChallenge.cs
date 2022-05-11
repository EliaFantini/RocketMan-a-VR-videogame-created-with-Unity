using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartRocketChallenge : MonoBehaviour
{
    
    //public SwitchControl switchControl;

    public ButtonLamp buttonLamp1;
    public ButtonLamp buttonLamp2;
    public ButtonLamp tabletButtonLamp;

    public JoystickControl joystickControl;
    public ButtonLamp joystickActiveLight;

    // Update is called once per frame
    public ButtonLamp finalLamp;
    void Update()
    {   
        //when two first challenger are done activate the joystick
        if(buttonLamp2.on && buttonLamp2.lightColor == ButtonLamp.eColor.Green  
        && tabletButtonLamp.on && tabletButtonLamp.lightColor == ButtonLamp.eColor.Green) {
            joystickControl.canMove = true;
            joystickActiveLight.on = true;
            joystickActiveLight.lightColor = ButtonLamp.eColor.Green;

        }
        if(buttonLamp1.on && buttonLamp1.lightColor == ButtonLamp.eColor.Green 
        && buttonLamp2.on && buttonLamp2.lightColor == ButtonLamp.eColor.Green 
        && tabletButtonLamp.on && tabletButtonLamp.lightColor == ButtonLamp.eColor.Green) {
            //start rocket
            finalLamp.on = true;
            GameManager.Instance.UpdateGameState(RiddlesProgress.RocketLaunched);
        } else {
            //Do nothing
        }
    }
}

