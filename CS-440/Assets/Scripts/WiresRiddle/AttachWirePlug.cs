using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachWirePlug : MonoBehaviour
{
    [SerializeField]
    public WiresRiddleController controller;
    public GameObject correctPlug;
    public Vector3 pluggedPosition;
    public Quaternion pluggedRotation;
    GameObject plug;
    AudioSource sound;
    public bool isPlugged;
    public bool ciao;
    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPlugged = false;
        ciao = false;
    }


    private void OnTriggerEnter(Collider other) {

        if(!isPlugged && (other.gameObject.tag == "Plug" || gameObject.tag == "PowerSocket")) {
            isPlugged = true;
            other.GetComponent<Rigidbody>().useGravity = false;
            other.transform.position = pluggedPosition;
            other.transform.rotation = pluggedRotation;
            plug = other.gameObject;
            sound.Play();
            
            if (gameObject.tag == "PowerSocket")
            {
                controller.turnOnScreen();
            }
            if (correctPlug != null && other.gameObject == correctPlug)
            {
                controller.addCorrectlyPlugged();
            }
        }
        
        

    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlugged && plug == other.gameObject)
        {
            plug = null;
            isPlugged = false;
            ciao = true;
            other.GetComponent<Rigidbody>().useGravity = true;
            sound.Play();
            if (gameObject.tag == "PowerSocket")
            {
                controller.turnOffScreen();
            }
            if (correctPlug != null && other.gameObject == correctPlug)
            {
                controller.removeCorrectlyPlugged();
            }
        }
    }
}
