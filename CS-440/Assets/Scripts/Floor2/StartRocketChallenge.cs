using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartRocketChallenge : MonoBehaviour
{
    
    //public SwitchControl switchControl;

    public ButtonLamp buttonLamp1;
    public ButtonLamp buttonLamp2;
    public bool done = false;
    public JoystickControl joystickControl;
    public ButtonLamp joystickActiveLight;

    void Update()
    {   
        //when two first challenger are done activate the joystick
        if(buttonLamp2.on && buttonLamp2.lightColor == ButtonLamp.eColor.Green  
        && buttonLamp1.on && buttonLamp1.lightColor == ButtonLamp.eColor.Green && !done) {
            joystickControl.canMove = true;
            joystickActiveLight.on = true;
            joystickActiveLight.lightColor = ButtonLamp.eColor.Red;
            

        }
        else if (!done)
        {
            joystickControl.canMove = false;
            joystickActiveLight.on = false;
        }

        if(buttonLamp1.on && buttonLamp1.lightColor == ButtonLamp.eColor.Green 
        && buttonLamp2.on && buttonLamp2.lightColor == ButtonLamp.eColor.Green
        && joystickActiveLight.on && joystickActiveLight.lightColor == ButtonLamp.eColor.Green && !done) 
        {
            done = true;
            
        }
        
    }
}

