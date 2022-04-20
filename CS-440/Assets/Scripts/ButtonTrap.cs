using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonTrap : MonoBehaviour
{
    [SerializeField]
    public GameObject Door;
    public GameObject button;
    private Animator anim;
    GameObject presser;
    AudioSource sound;
    bool isPressed;
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
        anim = Door.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        if(!isPressed) {
            button.transform.position = button.transform.position + new Vector3(0, -0.02f, 0);
            other.transform.position = other.transform.position + new Vector3(0, -0.02f, 0);
            presser = other.gameObject;
            onButtonPressed();
            sound.Play();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject == presser) {
            button.transform.position = button.transform.position + new Vector3(0, 0.02f, 0);
            onButtonReleased();
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
}

