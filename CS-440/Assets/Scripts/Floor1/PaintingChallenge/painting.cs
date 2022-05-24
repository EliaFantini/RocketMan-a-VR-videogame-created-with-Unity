using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class painting : MonoBehaviour
{
    [SerializeField]
    public Vector3 onWallPosition;
    public Quaternion onWallRotation;

    public AudioSource sound;
    public patternLock exitTablet;
    private bool done = false;



    public void frameAttached()
    {
       
            if (GetComponent<OVRGrabbable>().isGrabbed)
            {
                GetComponent<OVRGrabbable>().grabbedBy.ForceRelease(GetComponent<OVRGrabbable>());
            }
            GetComponent<OVRGrabbable>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;

            StartCoroutine(MoveOverSpeed(gameObject, onWallPosition, 1f));

            transform.rotation = onWallRotation;
            sound.Play();

            exitTablet.paintingPlacedCorrectly();
            GameManager.Instance.UpdateGameState(RiddlesProgress.FrameAttached);

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!done)
        {
            done = true;
            GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
            
            GameManager.Instance.UpdateGameState(RiddlesProgress.FrameFallen);
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

}
