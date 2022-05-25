using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_2 : MonoBehaviour
{
    private AudioSource source;
    public AudioClip buttonSound;
    // Start is called before the first frame update

    private bool on = false;
    private bool buttonHit = false;
    private GameObject button;

    private float buttonDownDistance = 0.025f;
    private float buttonReturnSpeed = 0.001f;
    private float buttonOriginalY; 

    //private Light spotLight;
    public ButtonLamp buttonLamp;

    private float buttonHitAgainTime = 0.5f;
    private float canHitAgain; 


    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        //get button and position
        button = transform.GetChild(0).gameObject; //first child of object
        buttonOriginalY = button.transform.position.y;

        //button lamp starts on off
        buttonLamp.on = false;
    }

    // Update is called once per frame
    void Update() {
        if(buttonHit == true) {
            source.PlayOneShot(buttonSound);
            buttonHit = false;

            on = !on; //switch button on and off

            button.transform.position = new Vector3(button.transform.position.x, 
                                                    button.transform.position.y - buttonDownDistance,
                                                    button.transform.position.z);
            if(on) {
                buttonLamp.on = true;
            } else {
                buttonLamp.on = false;
            }
        }

        if(button.transform.position.y < buttonOriginalY) {
            button.transform.position += new Vector3(0, buttonReturnSpeed, 0);
        }
    }

    private void OnTriggerEnter(Collider other) {
        //avoid unwanted clicks 
        if(other.CompareTag("PlayerHand") && canHitAgain < Time.time) {
            canHitAgain = Time.time + buttonHitAgainTime;
            buttonHit = true;
        }
    }
}
