using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class handling the plugging of the wires for both the wires riddles and the power socket, attached to the plugs
/// </summary> 
public class AttachWirePlug : MonoBehaviour
{
    [SerializeField]
    public WiresRiddleController controller;
    public Vector3 pluggedPosition;
    public Quaternion pluggedRotation;
    public Vector3 initialPosition;
    public Quaternion initialRotation;
    public int outletId;
    GameObject plug;
    AudioSource sound;
    public bool isPlugged;
    
    
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPlugged = false;
        
    }

    /// <summary>
    /// On triggering of a collider, check if it is a wire and plug it in if so
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (!isPlugged && (other.gameObject.tag == "Plug" || other.gameObject.tag == "PowerSocket"))
        {

            isPlugged = true;

            // Force release the wire
            if (other.gameObject.GetComponent<OVRGrabbable>().isGrabbed)
            {
                other.gameObject.GetComponent<OVRGrabbable>().grabbedBy.ForceRelease(other.gameObject.GetComponent<OVRGrabbable>());
            }
            other.gameObject.GetComponent<OVRGrabbable>().enabled = false;
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;

            // Move the wire into plugged position
            StartCoroutine(MoveOverSpeed(other.gameObject, pluggedPosition, 1f));

            other.transform.rotation = pluggedRotation;
            plug = other.gameObject;
            sound.Play();

            // Turn on screen if the wire is the power cable of the computer
            if (other.gameObject.tag == "PowerSocket")
            {
                controller.turnOnScreen();
            }

            // Inform the controller of the wire riddles a wire has been plugged in
            if (other.gameObject.tag == "Plug")
            {
                controller.plugged(other.gameObject, outletId);
            }
        }
    }

    /// <summary>
    /// Move the wire into plugging position
    /// </summary>
    /// <param name="objectToMove">Object to move, the wire</param>
    /// <param name="end">Plugging position</param>
    /// <param name="speed">Speed</param>
    /// <returns></returns>
    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed)
    {
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Detach the wires
    /// </summary>
    public void detach()
    {
        StartCoroutine(MoveBackOverSpeed(plug, initialPosition, 1f));    
    }

    /// <summary>
    /// Used to detach the wires, move the wire back into initial position
    /// </summary>
    /// <param name="objectToMove">Object to move, the wires</param>
    /// <param name="end">End position</param>
    /// <param name="speed">Speed</param>
    /// <returns></returns>
    public IEnumerator MoveBackOverSpeed(GameObject objectToMove, Vector3 end, float speed)
    {
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            
        }
        yield return new WaitForSeconds(1);
        objectToMove.GetComponent<OVRGrabbable>().enabled = true;
        objectToMove.GetComponent<BoxCollider>().enabled = true;
        objectToMove.GetComponent<Rigidbody>().isKinematic = false;

        objectToMove.transform.rotation = initialRotation;
        plug = null;
        sound.Play();
        isPlugged = false;
    }

}



