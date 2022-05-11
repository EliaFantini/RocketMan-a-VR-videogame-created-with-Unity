using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Update is called once per frame
    void Update()
    {   
        b1.on = true;
        b2.on = true;
        b3.on = true;
        b1.lightColor = ButtonLamp.eColor.Red;
        b2.lightColor = ButtonLamp.eColor.Red;
        b3.lightColor = ButtonLamp.eColor.Red;

        if(tab1.number == 7) {
            b1.lightColor = ButtonLamp.eColor.Green;
        } else {
            b1.lightColor = ButtonLamp.eColor.Red;
        }
        if(tab2.number == 5) {
            b2.lightColor = ButtonLamp.eColor.Green;
        } else {
            b2.lightColor = ButtonLamp.eColor.Red;
        }
        if(tab3.number == 8) {
            b3.lightColor = ButtonLamp.eColor.Green;
        } else {
            b3.lightColor = ButtonLamp.eColor.Red;
        }
        
        finalLamp.on = true;
        //if all numbers are correct light up the final lamp
        if(b1.on && b2.on && b3.on) {
            finalLamp.lightColor = ButtonLamp.eColor.Green;
        }
        if(b1.on || b2.on || b3.on) {
            finalLamp.lightColor = ButtonLamp.eColor.Yellow;
        } else {
            finalLamp.lightColor = ButtonLamp.eColor.Red;
        }

        
    }
}
