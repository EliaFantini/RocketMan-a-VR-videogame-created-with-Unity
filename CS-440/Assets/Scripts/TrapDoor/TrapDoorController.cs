using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoorController : MonoBehaviour
{
    private GameObject trapDoor;
    private bool isOpen;
    private Animator anim;
    public AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        trapDoor = gameObject;
        isOpen = true;
        anim = trapDoor.GetComponent<Animator>();
    }

    public void trapDoorButtonPressed()
    {
        if (!isOpen)
        {
            isOpen = true;
        }
        else
        {
            isOpen = false;
        }
        sound.time = 1f;
        sound.Play();
        anim.SetBool("button_pressed", isOpen);
    }
    public void setIsOpen(bool isOpenSetter)
    {
        isOpen = isOpenSetter;
        sound.time = 1f;
        sound.Play();
        anim.SetBool("button_pressed", isOpen);
    }
   
}
