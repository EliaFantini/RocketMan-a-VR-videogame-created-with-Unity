using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControl : MonoBehaviour
{
    private AudioSource source;
    public AudioClip switchSound; 

    private bool on = false;

    private float canHitAgain;
    private float switchHitAgainTime = 0.5f;
    private bool switchHit = false;
    private float switchRotation = -100;
    private GameObject switchBase;

    public ButtonLamp buttonLamp;

    private Quaternion original_transform;
    
    // Start is called before the first frame update
    void Start()
    {   
        source = gameObject.AddComponent<AudioSource>();
        switchBase = transform.GetChild(0).gameObject; //get cylinder(switch base)
        original_transform = Quaternion.Euler(transform.eulerAngles.x,
                                                        transform.eulerAngles.y, transform.eulerAngles.z);
        //turn off spotlight
        buttonLamp.on = false;
        
    }

    // Update is called once per frame
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
                buttonLamp.on = true;
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x + switchRotation,
                                                        transform.eulerAngles.y, transform.eulerAngles.z);
            } else {
                buttonLamp.on = false;
                transform.rotation = original_transform;
                                                        
                                                        
            }       
        }
    }

    private void OnTriggerEnter(Collider other) {
        //avoid unwanted clicks 
        if(other.CompareTag("PlayerHand") && canHitAgain < Time.time) {
            canHitAgain = Time.time + switchHitAgainTime;
            switchHit = true;
        }
    }
}
