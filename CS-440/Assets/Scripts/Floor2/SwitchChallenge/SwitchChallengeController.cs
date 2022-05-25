using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if the corrects switch are on and off according to the combination on the wall
/// When finished, light green lamp and update game state
/// </summary>
public class SwitchChallengeController : MonoBehaviour
{
    public SwitchControl switch1;
    public SwitchControl switch2;
    public SwitchControl switch3;
    public SwitchControl switch4;
    public SwitchControl switch5;
    public SwitchControl switch6;
    public ButtonLamp finalLamp;

    private bool riddleDone = false;


    void Update()
    {
        if(switch1.on == true && switch2.on == false &&
        switch3.on == true && switch4.on == true && switch5.on == false && switch6.on == true) {
            finalLamp.on = true;
            if (!riddleDone)
            {
                riddleDone = true;       
                GameManager.Instance.UpdateGameState(RiddlesProgress.SwitchesRiddle);
            }
            
        } else {
            finalLamp.on =false;
        }
    }
}
