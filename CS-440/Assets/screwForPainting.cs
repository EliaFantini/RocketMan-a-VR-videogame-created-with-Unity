using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
