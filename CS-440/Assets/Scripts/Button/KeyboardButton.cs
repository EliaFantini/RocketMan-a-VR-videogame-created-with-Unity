using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Class for the keyboard buttons
/// Delegate the button listener to the keyboard class
/// The name of the button correspond to the number, it is passed as an argument
/// </summary>
public class KeyboardButton : MonoBehaviour
{
    Keyboard keyboard;
    TextMeshProUGUI buttonText;

    void Start()
    {
        keyboard = GetComponentInParent<Keyboard>();
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
