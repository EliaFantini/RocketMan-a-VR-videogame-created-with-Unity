using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Keyboard_Tuto : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField inputField;
    public int cur_screen_number = 0;
    public int number_of_screens = 7;

    public void InsertChar(string c) {
        new WaitForSeconds(1);
        if(c == "1" &&  cur_screen_number < number_of_screens) { //increment
             cur_screen_number =  cur_screen_number + 1;
        }
        else if(c == "2" &&  cur_screen_number > 0) { //decrement
             cur_screen_number =  cur_screen_number -1;
        }
        inputField.text =  cur_screen_number.ToString();;
        waiter(); //avoid double button clicks
    }

    IEnumerator waiter()
    {
        //Wait for 1 seconds
        yield return new WaitForSecondsRealtime(2);
    }
    
}
