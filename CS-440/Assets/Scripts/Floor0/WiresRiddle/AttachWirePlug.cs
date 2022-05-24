using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void OnTriggerEnter(Collider other)
    {
        if (!isPlugged && (other.gameObject.tag == "Plug" || other.gameObject.tag == "PowerSocket"))
        {

            isPlugged = true;
            if (other.gameObject.GetComponent<OVRGrabbable>().isGrabbed)
            {
                other.gameObject.GetComponent<OVRGrabbable>().grabbedBy.ForceRelease(other.gameObject.GetComponent<OVRGrabbable>());
            }
            other.gameObject.GetComponent<OVRGrabbable>().enabled = false;
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;

            StartCoroutine(MoveOverSpeed(other.gameObject, pluggedPosition, 1f));

            other.transform.rotation = pluggedRotation;
            plug = other.gameObject;
            sound.Play();

            if (other.gameObject.tag == "PowerSocket")
            {
                controller.turnOnScreen();
            }
            if (other.gameObject.tag == "Plug")
            {
                controller.plugged(other.gameObject, outletId);
            }


        }
    }

    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed)
    {
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void detach()
    {

        StartCoroutine(MoveBackOverSpeed(plug, initialPosition, 1f));
        
    }

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



