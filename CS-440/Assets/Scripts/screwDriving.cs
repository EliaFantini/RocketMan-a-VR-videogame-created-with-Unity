using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screwDriving : MonoBehaviour
{
    private bool descrewing = false;
    private int time = 0;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {}
    // Update is called once per frame
    void Update()
    {
        if(descrewing)
        {
            rb.transform.position = rb.transform.position + new Vector3(0,0.002f,0);
            rb.transform.Rotate(0,0,15,Space.Self);
            time += 1;
            if(time == 120)
            {
                rb.isKinematic = false;
                rb.AddForce(1, 1, 0);
                descrewing = false;
                time = 0;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "screw")
        {
            rb = other.GetComponent<Rigidbody>();
            descrewing = true;
        }
    }
}