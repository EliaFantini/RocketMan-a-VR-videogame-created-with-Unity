using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedTickColliding : MonoBehaviour
{

    public wrench wrenchRotated;
    private bool done = false;
   


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "redTick" && !done)
        {
            wrenchRotated.boltRotated();
            done = true;
        }
    }
}
