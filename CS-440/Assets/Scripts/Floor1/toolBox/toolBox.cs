using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to add and remove object from the box
/// </summary>
public class toolBox : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> boxTools;

    /// <summary>
    /// If an object stay on trigger, meaning if and object is in the box, add the object to the box by setting it as a child of the box
    /// </summary>
    /// <param name="other">Other object</param>
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponent<OVRGrabbable>() != null && !boxTools.Contains(other.gameObject))
        {
            boxTools.Add(other.gameObject);
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            other.gameObject.transform.parent = transform;
        }
    }

    /// <summary>
    /// If an object which was contained by the box exit the it, removes this object from the children of the box
    /// </summary>
    /// <param name="other"></param>
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
