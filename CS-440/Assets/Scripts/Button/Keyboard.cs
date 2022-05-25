using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Class for the keyboard, insert and delete char on the display
/// </summary>
public class Keyboard : MonoBehaviour
{
    public TMP_InputField inputField;
    private bool open = false;

    public CapsuleDoor capsuleDoor;

    public void InsertChar(string c) {
        inputField.text += c;
        if(inputField.text.Equals("9344") && !open){
            capsuleDoor.open();
            open = true;
        }
    }
    public void DeleteChar() {
        if(inputField.text.Length > 0) {
            inputField.text = inputField.text.Substring(0, inputField.text.Length -1);
        }
    }
    public void InsertSpace() {
        inputField.text += " ";
    }

}
