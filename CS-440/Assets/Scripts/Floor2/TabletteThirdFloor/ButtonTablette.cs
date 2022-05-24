using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonTablette : MonoBehaviour
{
    TabletteThirdFloor keyboard;
    
    TextMeshProUGUI buttonText;
     
    void Start()
    {
        keyboard = GetComponentInParent<TabletteThirdFloor>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if(buttonText.text.Length == 1) {
            NameToButtonText();
            GetComponentInChildren<ButtonVR>().onPress.AddListener(
                delegate {keyboard.InsertChar(buttonText.text);}
            );
        }
        
    }
    public void NameToButtonText() {
        buttonText.text = gameObject.name;
    }
}
