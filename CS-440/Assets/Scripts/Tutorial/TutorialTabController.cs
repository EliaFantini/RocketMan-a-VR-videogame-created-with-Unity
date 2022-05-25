using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the tutorial tab
/// </summary>
public class TutorialTabController : MonoBehaviour
{
    private GameObject screen_displays;

    private GameObject cur_screen;

    public Keyboard_Tuto keyboard_Tuto;

    public Vector3 finalTabletPos;
    private int currNumber = 0;
    private bool numberChanged = false;

    private 
    void Start()
    {
        screen_displays = transform.GetChild(1).gameObject; //get gameObject containing display for screen
        cur_screen = screen_displays.transform.GetChild(currNumber).gameObject; //Get first display
        cur_screen.SetActive(true); //display it
    }

    // Update is called once per frame
    void Update()
    {
        if(currNumber != keyboard_Tuto.cur_screen_number)
        {
            currNumber = keyboard_Tuto.cur_screen_number;
            numberChanged = true;
        }
        //deactivate current display and activate display corresponding to tuto_Buttons.number
        //(this number changes when you click on the arrows on the screen)
        if(keyboard_Tuto.cur_screen_number == 11) {
            StartCoroutine(MoveOverSpeed(gameObject, finalTabletPos, 1f));
        }
        else if(numberChanged){
            cur_screen.SetActive(false);
            cur_screen = screen_displays.transform.GetChild(currNumber).gameObject;
            cur_screen.SetActive(true);
            numberChanged = false;
        }
        
    }

    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed)
    {
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        keyboard_Tuto.cur_screen_number = 10;
        keyboard_Tuto.number_of_screens = 10;
    }
}
