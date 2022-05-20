using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Keyboard_Tuto : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField inputField;
    public int number;
    private int number_of_screens = 3;

    public void InsertChar(string c) {
        if(c == "1" && number < number_of_screens) { //increment
            number = number + 1;
        }
        else if(c == "2" && number > 0) { //decrement
            number = number -1;
        }
        inputField.text = number.ToString();;
    }
    
}
