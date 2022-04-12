using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Keyboard : MonoBehaviour
{
    public TMP_InputField inputField;

    public CapsuleDoor capsuleDoor;

    public void InsertChar(string c) {
        inputField.text += c;
        if(inputField.text.Equals("1111")){
            capsuleDoor.open();
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
