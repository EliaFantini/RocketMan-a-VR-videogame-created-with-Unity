/**************************************************
Copyright : Copyright (c) RealaryVR. All rights reserved.
Description: Script for VR Button functionality.
***************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// General class for buttons 
/// </summary>
public class ButtonVR : MonoBehaviour
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

    /// <summary>
    /// On trigger enter, change button's position, make sound and invoke onPress
    /// </summary>
    /// <param name="other">Object colliding</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            sound.Play();
            isPressed = true;
        }
        StartCoroutine(WaitAndTrigger()); //automatically release after 5 seconds
    }

    /// <summary>
    /// On trigger exit, invoke release and change the button's position
    /// </summary>
    /// <param name="other">Object colliding</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser)
        {
            StartCoroutine(WaitToAvoidDoubleClicks());
            button.transform.localPosition = new Vector3(0, 0.015f, 0);
            onRelease.Invoke();
            isPressed = false;
        }
    }



    //Automatically release button after a few seconds to avoid glitch where buttons doesnt detect triggerExit
    public IEnumerator WaitAndTrigger()
    {
        yield return new WaitForSeconds(5);
        button.transform.localPosition = new Vector3(0, 0.015f, 0);
        isPressed = false;
    }

     public IEnumerator WaitToAvoidDoubleClicks()
    {
        yield return new WaitForSeconds(2);
       
    }

}
