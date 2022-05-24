using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toolBox : MonoBehaviour
{

    [SerializeField]
    public List<GameObject> boxTools;
    public Vector3 boxBoundaries;


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponent<OVRGrabbable>() != null && !boxTools.Contains(other.gameObject))
        {
            boxTools.Add(other.gameObject);
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            other.gameObject.transform.parent = transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (boxTools.Contains(other.gameObject))
        {
            boxTools.Remove(other.gameObject);
            if(other.gameObject.transform.parent == transform)
            {
                other.gameObject.transform.parent = null;
            }
           
        }
    }
}
