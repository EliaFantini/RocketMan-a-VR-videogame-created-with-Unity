using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the 2 red tick collides, calls on the wrench class so that the bolt stops
/// </summary>
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
