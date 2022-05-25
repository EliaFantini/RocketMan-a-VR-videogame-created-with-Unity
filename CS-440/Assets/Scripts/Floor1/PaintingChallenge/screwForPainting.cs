using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the screw to attach the painting
/// If it collides wit the frame, calls on the painting class to attach it
/// </summary>
public class screwForPainting : MonoBehaviour
{
    private bool isOnWall = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isOnWall && other.gameObject.tag == "painting")
        {
            isOnWall = true;
            other.GetComponent<painting>().frameAttached();
        }
    }


}
