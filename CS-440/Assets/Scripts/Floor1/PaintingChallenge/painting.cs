using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class for the painting
/// Takes cares of the detachment and placement of the painting
/// </summary>
public class painting : MonoBehaviour
{
    [SerializeField]
    public Vector3 onWallPosition;
    public Quaternion onWallRotation;

    public AudioSource sound;
    public patternLock exitTablet;
    private bool done = false;


    /// <summary>
    /// Attach the frame to the screw
    /// Force release it from the hand, placed it on the correct position and update the game state
    /// </summary>
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

    /// <summary>
    /// On collision, make the frame fall
    /// </summary>
    /// <param name="collision">Collision object</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (!done)
        {
            done = true;
            GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
            
            GameManager.Instance.UpdateGameState(RiddlesProgress.FrameFallen);
        }

    }

    /// <summary>
    /// Move the painting to the correct position
    /// </summary>
    /// <param name="objectToMove">The painting</param>
    /// <param name="end">End position</param>
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

}
