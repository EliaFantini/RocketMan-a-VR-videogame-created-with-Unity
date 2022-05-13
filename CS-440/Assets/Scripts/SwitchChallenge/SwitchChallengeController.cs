using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchChallengeController : MonoBehaviour
{
    
    //public SwitchControl switchControl;

    public SwitchControl switch1;
    public SwitchControl switch2;
    public SwitchControl switch3;
    public SwitchControl switch4;
    public SwitchControl switch5;
    public SwitchControl switch6;
    public ButtonLamp finalLamp;

    private bool riddleDone = false;

    // Update is called once per frame
    void Update()
    {
        if(switch1.on == true && switch2.on == false &&
        switch3.on == true && switch4.on == true && switch5.on == false && switch6.on == true) {
            if (!riddleDone)
            {
                riddleDone = true;

                finalLamp.on = true;
                GameManager.Instance.UpdateGameState(RiddlesProgress.SwitchesRiddle);
            }
            
        } else {
            finalLamp.on =false;
        }
    }
}
