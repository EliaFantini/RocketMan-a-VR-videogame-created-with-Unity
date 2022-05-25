using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Increment and decrement the number displayed on the tablet when the up and down arrows are pressed
/// It receives 1 or 2 as argument which corresponds to the up and down arrows.
/// </summary>
public class TabletteThirdFloor : MonoBehaviour
{
  
    public TMP_InputField inputField;
    public int number;

    public void InsertChar(string c) {
        if(c == "1" && number < 9) { //increment
            number = number + 1;
        }
        else if(c == "2" && number > 0) { //decrement
            number = number -1;
        }
        inputField.text = number.ToString();;   
    }
}

