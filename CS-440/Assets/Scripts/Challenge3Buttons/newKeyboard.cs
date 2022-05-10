using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newKeyboard : MonoBehaviour
{
    public ButtonLamp lamp; 
    
    public void InsertChar(string c) {
       
        lamp.lightColor = ButtonLamp.eColor.Blue;
    
    }
    public void DeleteChar() {
       lamp.lightColor = ButtonLamp.eColor.Red;
    }

    public void RemoveChar() {
        lamp.lightColor = ButtonLamp.eColor.Green;
    }
    
}
