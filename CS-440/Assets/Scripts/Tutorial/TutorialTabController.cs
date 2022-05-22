using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTabController : MonoBehaviour
{
    
    private GameObject screen_displays;

    private GameObject cur_screen;

    public Keyboard_Tuto keyboard_Tuto;


    private 
    void Start()
    {
        screen_displays = transform.GetChild(1).gameObject; //get gameObject containing display for screen
        cur_screen = screen_displays.transform.GetChild(keyboard_Tuto.cur_screen_number).gameObject; //Get first display
        cur_screen.SetActive(true); //display it
    }

    // Update is called once per frame
    void Update()
    {
        //deactivate current display and activate display corresponding to tuto_Buttons.number
        //(this number changes when you click on the arrows on the screen)
        // if(keyboard_Tuto.cur_screen_number == 7) destroy this object 
        cur_screen.SetActive(false);
        cur_screen = screen_displays.transform.GetChild(keyboard_Tuto.cur_screen_number).gameObject;
        cur_screen.SetActive(true);
    }
}
