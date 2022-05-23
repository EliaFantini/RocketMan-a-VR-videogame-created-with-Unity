using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UVLight : MonoBehaviour
{
    [SerializeField]
    public GameObject light;
    private bool on;
    private bool firstTimeGrabbed = true;
    // Start is called before the first frame update
    private OVRGrabber grabber;
    public AudioSource audioSource;

    void Start()
    {
        light.GetComponent<Light>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (gameObject.GetComponent<OVRGrabbable>().isGrabbed){
            if (firstTimeGrabbed)
            {
                firstTimeGrabbed = false;
                GameManager.Instance.UpdateGameState(RiddlesProgress.UVLightGrabbed);
            }
            /*
            grabber = gameObject.GetComponent<OVRGrabbable>().grabbedBy;
            
            if (grabber.m_controller == OVRInput.Controller.LTouch && OVRInput.GetDown(OVRInput.RawButton.X))
            {
                grabber.hapticPulse();
                light.GetComponent<Light>().enabled = !light.GetComponent<Light>().enabled;
                audioSource.Play();
            }
            else if (grabber.m_controller == OVRInput.Controller.RTouch && OVRInput.GetDown(OVRInput.RawButton.A))
            {
                audioSource.Play();
                grabber.hapticPulse();
                light.GetComponent<Light>().enabled = !light.GetComponent<Light>().enabled;
            }
            */


        }
    }
}
