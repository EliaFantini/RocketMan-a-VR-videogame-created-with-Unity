using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screwDriving : MonoBehaviour
{

    private int timeScrew = 0;
    private Rigidbody rb;
    private GameObject screw;
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
        if(Drill.GetComponent<OVRGrabbable>().isGrabbed)
        {
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger)> 0.5f){
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
        

        if(trigger){
            transform.Rotate(0,0,15,Space.Self);
            if(onscrew){
                rb.transform.position = rb.transform.position + new Vector3(0,0.002f,0);
                rb.transform.Rotate(0,0,-15,Space.Self);
                timeScrew +=1;
            }

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
            screw = other.gameObject;
            rb = other.GetComponent<Rigidbody>();
            onscrew = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == screw)
        {
            rb = null;
            onscrew = false;
        }
    }
}