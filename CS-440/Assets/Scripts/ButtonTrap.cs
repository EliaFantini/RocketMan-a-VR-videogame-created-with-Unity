using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum eColor
{
    Red,
    Yellow,
    Green,
    Blue,
}

public class ButtonTrap : MonoBehaviour
{
 

    [SerializeField]
    public GameObject Door;
    public GameObject button;
    public ButtonLamp buttonLamp;
    private Animator anim;
    GameObject presser;
    AudioSource sound;
    public bool isPressed;
    public bool enabled;
    public ButtonLamp lamp;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
        enabled = false;
        anim = Door.GetComponent<Animator>();

        lamp = button.GetComponent<ButtonLamp>();
    }

    private void OnTriggerEnter(Collider other) {
        if(!isPressed) {
            button.transform.position = button.transform.position + new Vector3(0, -0.02f, 0);
            other.transform.position = other.transform.position + new Vector3(0, -0.02f, 0);
            presser = other.gameObject;
            if (enabled)
            {
                onButtonPressed();
            }
            
            sound.Play();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject == presser) {
            button.transform.position = button.transform.position + new Vector3(0, 0.02f, 0);
            if (enabled)
            {
                onButtonReleased();
            }
            isPressed = false;
        }
    }
    private void onButtonPressed()
    {
        anim.SetBool("button_pressed", true);

    }
    private void onButtonReleased()
    {
        anim.SetBool("button_pressed", false);
    }
    public void enableButton()
    {
        enabled = true;
        lamp.on = true;
        lamp.lightColor = ButtonLamp.eColor.Green;
    }
}

