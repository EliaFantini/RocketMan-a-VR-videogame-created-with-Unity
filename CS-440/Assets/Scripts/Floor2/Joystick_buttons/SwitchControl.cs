using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control the switches, attached to each one of them
/// </summary>
public class SwitchControl : MonoBehaviour
{
    private AudioSource source;
    public AudioClip switchSound; 

    public bool on = false;

    private float canHitAgain;
    private float switchHitAgainTime = 0.5f;
    private bool switchHit = false;
    private float switchRotation = -100;
    private GameObject switchBase;

    public ButtonLamp buttonLamp;

    private Quaternion original_transform;
    
    /// <summary>
    /// Set up switches
    /// </summary>
    void Start()
    {   
        source = gameObject.AddComponent<AudioSource>();
        switchBase = transform.GetChild(0).gameObject; //get cylinder(switch base)
        original_transform = Quaternion.Euler(transform.eulerAngles.x,
                                                        transform.eulerAngles.y, transform.eulerAngles.z);
    }

    /// <summary>
    /// Change switches position when hit
    /// </summary>
    void Update()
    {
        if(switchHit == true) {
            //play sound
            source.PlayOneShot(switchSound);
            switchHit = false;
            //switch between on and off
            on = !on;

            //move lever
            if(on == true) {
                
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x + switchRotation,
                                                        transform.eulerAngles.y, transform.eulerAngles.z);
            } else {
                
                transform.rotation = original_transform;                                                  
            }       
        }
    }

    /// <summary>
    /// Checks when the switch is hit, if it is hit by the hand change switchHit to true
    /// There is a time window to avoid unwanted click
    /// </summary>
    /// <param name="other">Object colliding</param>
    private void OnTriggerEnter(Collider other) {
        //avoid unwanted clicks 
        if(other.CompareTag("PlayerHand") && canHitAgain < Time.time) {
            canHitAgain = Time.time + switchHitAgainTime;
            switchHit = true;
            other.gameObject.GetComponentInChildren<OVRGrabber>().hapticPulse();
        }
    }
}
