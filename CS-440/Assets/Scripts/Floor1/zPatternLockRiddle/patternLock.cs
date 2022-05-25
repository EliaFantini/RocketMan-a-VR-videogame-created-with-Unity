using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller of the pattern lock, used for both the Z pattern riddle and the painting riddle
/// </summary>
public class patternLock : MonoBehaviour
{
    [SerializeField]
    public GameObject[] objectsNotGrabbable;
    public GameObject[] objectsGrabbable;
    public Material matZlock;
    public Material matlock;
    public Material greenExit;
    public Material redWarn;
    public AudioSource audioSource;
    public AudioClip wallGoingDown;
    public GameObject slidingWall;
    private bool paintingPlaced = false;
    public bool fireExtinguished = false;

    /// <summary>
    /// Play sound and make the tool box fall when the correct pattern is drawn
    /// This is done by switching a non-grabbable, kinematic toolbox with a grabbable, non-kimematic one
    /// </summary>
    public void onCorrectPattern()
    {
        Vector3 grabbablePos = objectsNotGrabbable[0].transform.position;
        objectsNotGrabbable[0].transform.position = objectsGrabbable[0].transform.position;
        objectsGrabbable[0].transform.position = grabbablePos;
        audioSource.Play();
        GameManager.Instance.UpdateGameState(RiddlesProgress.OpenBoxSign);
    }

    /// <summary>
    /// On trigger enter, check if it the light collider, if yes, changes the material to make the Z appear
    /// </summary>
    /// <param name="other">Other collider</param>
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "uvLight")
        {
            if (other.GetComponent<Light>().enabled)
            {
                //Make the Z appear
                gameObject.GetComponent<MeshRenderer>().material = matZlock;
            }
        }

        if (paintingPlaced && fireExtinguished)
        {
            // Move slinding wall
            paintingPlaced = false;
            Vector3 finalSlidePosition = slidingWall.transform.position;
            finalSlidePosition.y = finalSlidePosition.y - 4f;
            StartCoroutine(slideWall(slidingWall, finalSlidePosition,1f));
            AudioSource.PlayClipAtPoint(wallGoingDown, slidingWall.transform.position);
            GameManager.Instance.UpdateGameState(RiddlesProgress.ExitPushed);
        }

        else if (paintingPlaced && !fireExtinguished)
        {
            // Display red warning to extinguish the fire
            StartCoroutine(redWarning());
        }
            
    }
    
    /// <summary>
    /// On trigger exit, changes the material if it was the light collider exiting
    /// </summary>
    /// <param name="other">Other collider</param>
    private void OnTriggerExit(Collider other) {
        if(other.tag == "uvLight"){
            gameObject.GetComponent<MeshRenderer>().material = matlock;
        }
    }

    /// <summary>
    /// Change screen to green, called when the painting is placed correctly
    /// </summary>
    public void paintingPlacedCorrectly()
    {
        paintingPlaced = true;
        gameObject.GetComponent<MeshRenderer>().material = greenExit;
    }

    /// <summary>
    /// Slide the wall down
    /// </summary>
    /// <param name="objectToMove">The wall</param>
    /// <param name="end">End position of the wall</param>
    /// <param name="speed">Speed of the movement</param>
    /// <returns></returns>
    public IEnumerator slideWall(GameObject objectToMove, Vector3 end, float speed)
    {
        yield return new WaitForSeconds(1);
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Destroy(objectToMove);
    }

    /// <summary>
    /// Display red warning
    /// </summary>
    /// <returns></returns>
    public IEnumerator redWarning()
    {
        gameObject.GetComponent<MeshRenderer>().material = redWarn;
        yield return new WaitForSeconds(3);
        gameObject.GetComponent<MeshRenderer>().material = greenExit;
    }

}
