using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeObjectsInside : MonoBehaviour
{
    public Rigidbody[] objectsInside;


    // Update is called once per frame
    void Update()
    {
        if (GetComponent<OVRGrabbable>().isGrabbed)
        {
            foreach(Rigidbody objectInside in objectsInside)
            {
                objectInside.isKinematic = true;
            }
        }
        else
        {
            foreach (Rigidbody objectInside in objectsInside)
            {
                if(objectInside.isKinematic) objectInside.isKinematic = false;
            }
        }
    }
}
