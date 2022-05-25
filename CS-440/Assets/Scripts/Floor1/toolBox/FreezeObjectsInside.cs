using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class which freezes the object inside the box
/// </summary>
public class FreezeObjectsInside : MonoBehaviour
{
    public Rigidbody[] objectsInside;

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
