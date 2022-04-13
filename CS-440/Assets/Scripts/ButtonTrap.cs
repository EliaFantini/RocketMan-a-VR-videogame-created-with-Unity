using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonTrap : MonoBehaviour
{
   
   public GameObject button;
   public UnityEvent onPress;
   public UnityEvent onRelease;
   GameObject presser;
   AudioSource sound;
   bool isPressed;
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(!isPressed) {
            button.transform.position = button.transform.position + new Vector3(0, -0.02f, 0);
            other.transform.position = other.transform.position + new Vector3(0, -0.02f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            sound.Play();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject == presser) {
            button.transform.position = button.transform.position + new Vector3(0, 0.02f, 0);
            onRelease.Invoke();
            isPressed = false;
        }
    }
}

