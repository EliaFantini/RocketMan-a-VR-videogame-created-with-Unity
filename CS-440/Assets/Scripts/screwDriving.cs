using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screwDriving : MonoBehaviour
{
    private int timeDrill = 1000;
    private int timeScrew = 0;
    private Rigidbody rb;
    private bool onscrew = false;
    private bool trigger = false;
    [SerializeField]
    public GameObject Drill;
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if(Drill.GetComponent<OVRGrabbable>().isGrabbed){
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger)> 0.5f){
                trigger = true;
                timeDrill = 0;
            }
        }
        else
            trigger = false;

        if(trigger || timeDrill < 120){
            transform.Rotate(0,0,15,Space.Self);
            if(onscrew){
                rb.transform.position = rb.transform.position + new Vector3(0,0.002f,0);
                rb.transform.Rotate(0,0,-15,Space.Self);
                timeScrew +=1;
            }
            timeDrill += 1;
        }

        if(timeScrew >= 120)
        {
            if(onscrew){
                rb.isKinematic = false;
                rb.AddForce(1, 1, 0);
                onscrew = false;
            }
            timeScrew = 0;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "screw")
        {
            rb = other.GetComponent<Rigidbody>();
            onscrew = true;
        }
    }
}