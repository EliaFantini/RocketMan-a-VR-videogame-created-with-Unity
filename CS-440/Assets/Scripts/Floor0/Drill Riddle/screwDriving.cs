using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class attached to the drill
/// When pressing the index trigger with the drill in hand, the drill turns on
/// On collision of the drill with a screws, it allows the screws to be removed by turning the drill on
/// </summary>
public class screwDriving : MonoBehaviour
{

    private int timeScrew = 0;
    private Rigidbody rb;
    private GameObject screw;
    private bool onscrew = false;
    private bool trigger = false;
    [SerializeField]
    public GameObject Drill;
    public AudioSource audio;
    private int screwCount = 0;
    private bool riddleFinished = false;

    void Update()
    {
        // If grabbed, looks if the index trigger is pressed
        if(Drill.GetComponent<OVRGrabbable>().isGrabbed)
        {
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger)> 0.5f || OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f)
            {
                trigger = true;
            }
            else
            {
                trigger = false;
            }
        }
        else
        {
            trigger = false;
        }
        
        // If the index trigger is pressed, starts the drill and play sound and vibration
        if(trigger){
            transform.Rotate(0,0,15,Space.Self);
            if (audio.time > 3f)
            {
                audio.Stop();
            }
            if (!audio.isPlaying)
            {
                audio.time = 1f;
                audio.Play();
            }
            OVRInput.SetControllerVibration(0.3f, 0.3f, Drill.GetComponent<OVRGrabbable>().grabbedBy.m_controller);
            // If on the screw, rotate the screw
            if (onscrew){
                rb.transform.position = rb.transform.position + new Vector3(0,0.002f,0);
                rb.transform.Rotate(0,0,-15,Space.Self);
                timeScrew +=1;
                
            }

        }
        // Stop audio and vibration
        else
        {
            audio.Stop();
            OVRInput.SetControllerVibration(0.0f, 0.0f, Drill.GetComponent<OVRGrabbable>().grabbedBy.m_controller);
        }

        // After the screw has been descrewed enough, it removes it with addforce and make it kinematic
        if(timeScrew >= 120)
        {
            if(onscrew){
                rb.isKinematic = false;
                rb.AddForce(1, 1, 0);
                onscrew = false;
                if (screwCount < 2)
                {
                    screwCount += 1;
                }
                else if (!riddleFinished)
                {
                    riddleFinished = true;
                    GameManager.Instance.UpdateGameState(RiddlesProgress.ScrewsRemoved);
                }
                
            }
            timeScrew = 0;
            
        }
    }

    /// <summary>
    /// Looks if the colliding object is a screw
    /// </summary>
    /// <param name="other">Object colliding</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "screw")
        {
            screw = other.gameObject;
            rb = other.GetComponent<Rigidbody>();
            onscrew = true;
        }
    }
    /// 
    /// <summary>
    /// Looks if the exiting object is a screw
    /// </summary>
    /// <param name="other">Object colliding</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == screw)
        {
            rb = null;
            onscrew = false;
        }
    }
}