using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Delegates the arrows buttons listener to TabletteThirdFloor
/// One button is named "1" and the other "2", this name is passed on as an argument to InsertChar
/// The "1" button is the up arrow, the "2" is the down arrow
/// </summary>
public class ButtonTablette : MonoBehaviour
{
    TabletteThirdFloor keyboard;
    TextMeshProUGUI buttonText;
     
    void Start()
    {
        keyboard = GetComponentInParent<TabletteThirdFloor>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if(buttonText.text.Length == 1) {
            // Changes buttonText according to the name of the button
            NameToButtonText();
            GetComponentInChildren<ButtonVR>().onPress.AddListener(
                //Delegate button listener to TabletteThridFloor with the button text as argument
                delegate {keyboard.InsertChar(buttonText.text);}
            );
        }  
    }
    
    /// <summary>
    /// Changes buttonText according to the name of the gameObject
    /// </summary>
    public void NameToButtonText() {
        buttonText.text = gameObject.name;
    }
}
