using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
