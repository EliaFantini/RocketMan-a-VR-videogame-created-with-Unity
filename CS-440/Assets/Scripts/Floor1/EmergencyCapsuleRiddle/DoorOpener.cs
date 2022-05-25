using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Open the door for the emergency capsule when pressing the button
/// It uses a boolean to trigger a state change in the door animator
/// </summary>
public class DoorOpener : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void onButtonPressed()
    {
        anim.SetBool("button_pressed", true);
    }
    public void onButtonReleased()
    {
        anim.SetBool("button_pressed", false);
    }
}
