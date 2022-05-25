using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Class for the tutorial button, delegate the listener to the keyboard_tuto class
/// The name of the button correspond to the text of the button
/// </summary>
public class Tuto_button_behavior : MonoBehaviour
{
    // Start is called before the first frame update
    Keyboard_Tuto keyboard;
    
    TextMeshProUGUI buttonText;
     

    void Start()
    {
        keyboard = GetComponentInParent<Keyboard_Tuto>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if(buttonText.text.Length == 1) {
            NameToButtonText();
            GetComponentInChildren<ButtonVR>().onRelease.AddListener(
                delegate {keyboard.InsertChar(buttonText.text);}
            );
        }
        
    }
    public void NameToButtonText() {
        buttonText.text = gameObject.name;
    }
}
