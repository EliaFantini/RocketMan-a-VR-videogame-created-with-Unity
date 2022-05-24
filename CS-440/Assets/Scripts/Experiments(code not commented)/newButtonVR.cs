using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Button3 : MonoBehaviour
{
 
    public GameObject button;
    AudioSource sound;
    bool isPressed;

    public Challenge3press controller;


    void Start()
    {
        controller = GetComponent<Challenge3press>();
        sound = GetComponent<AudioSource>();
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        button.transform.localPosition = new Vector3(0, 0.003f, 0);
        sound.Play();
        isPressed = true;
        controller.button3Press();
    }

    private void OnTriggerExit(Collider other)
    {
        button.transform.localPosition = new Vector3(0, 0.015f, 0);
        isPressed = false;
        controller.button3Press();
    }

    public bool getIsPressed() {
        return isPressed;
    }
}