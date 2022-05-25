using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for the numeric pad riddles
/// Changes the color of the lamps according to the numbers
/// When the riddles is complete, light on the green light and update the game state
/// </summary>
public class TabletteChallengeThirdFloorController : MonoBehaviour
{
    // Start is called before the first frame update
    public ButtonLamp b1;
    public ButtonLamp b2;
    public ButtonLamp b3;

    public TabletteThirdFloor tab1;
    public TabletteThirdFloor tab2;
    public TabletteThirdFloor tab3;

    public ButtonLamp finalLamp;
    private bool riddleDone = false;
    
    // Update is called once per frame
    void Update()
    {
        b1.on = true;
        b2.on = true;
        b3.on = true;
        b1.lightColor = ButtonLamp.eColor.Red;
        b2.lightColor = ButtonLamp.eColor.Red;
        b3.lightColor = ButtonLamp.eColor.Red;
        // Changes colors
        switch(tab1.number)
        {
            case 1:
                b1.lightColor = ButtonLamp.eColor.Cyan;
                break;
            case 2:
                b1.lightColor = ButtonLamp.eColor.Grey;
                break;
            case 4:
                b1.lightColor = ButtonLamp.eColor.Magenta;
                break;
            case 6:
                b1.lightColor = ButtonLamp.eColor.Black;
                break;
            case 7:
                b1.lightColor = ButtonLamp.eColor.White;
                break;
            case 8:
                b1.lightColor = ButtonLamp.eColor.Red;
                break;
            
            case 9:
                b1.lightColor = ButtonLamp.eColor.Yellow;
                break;
            case 3:
                b1.lightColor = ButtonLamp.eColor.Green;
                break;
            case 5:
                b1.lightColor = ButtonLamp.eColor.Blue;
                break;
            default:
                b1.lightColor = ButtonLamp.eColor.Red;
                break;
        }
        switch (tab2.number)
        {
            case 1:
                b2.lightColor = ButtonLamp.eColor.Cyan;
                break;
            case 2:
                b2.lightColor = ButtonLamp.eColor.Grey;
                break;
            case 4:
                b2.lightColor = ButtonLamp.eColor.Magenta;
                break;
            case 6:
                b2.lightColor = ButtonLamp.eColor.Black;
                break;
            case 7:
                b2.lightColor = ButtonLamp.eColor.White;
                break;
            case 8:
                b2.lightColor = ButtonLamp.eColor.Red;
                break;

            case 9:
                b2.lightColor = ButtonLamp.eColor.Yellow;
                break;
            case 3:
                b2.lightColor = ButtonLamp.eColor.Green;
                break;
            case 5:
                b2.lightColor = ButtonLamp.eColor.Blue;
                break;
            default:
                b2.lightColor = ButtonLamp.eColor.Red;
                break;
        }
        switch (tab3.number)
        {
            case 1:
                b3.lightColor = ButtonLamp.eColor.Cyan;
                break;
            case 2:
                b3.lightColor = ButtonLamp.eColor.Grey;
                break;
            case 4:
                b3.lightColor = ButtonLamp.eColor.Magenta;
                break;
            case 6:
                b3.lightColor = ButtonLamp.eColor.Black;
                break;
            case 7:
                b3.lightColor = ButtonLamp.eColor.White;
                break;
            case 8:
                b3.lightColor = ButtonLamp.eColor.Red;
                break;

            case 9:
                b3.lightColor = ButtonLamp.eColor.Yellow;
                break;
            case 3:
                b3.lightColor = ButtonLamp.eColor.Green;
                break;
            case 5:
                b3.lightColor = ButtonLamp.eColor.Blue;
                break;
            default:
                b3.lightColor = ButtonLamp.eColor.Red;
                break;
        }

        if (tab1.number == 9 && tab2.number == 3 && tab3.number == 5)
        {
            //Riddles complete, lit up the green light
            finalLamp.lightColor = ButtonLamp.eColor.Green;
            finalLamp.on = true;
            if (!riddleDone)
            {
                riddleDone = true;
                GameManager.Instance.UpdateGameState(RiddlesProgress.ThreeDigitsCode);
            }
        }
        else
        {
            finalLamp.on = false;
        }

        
    }
}
