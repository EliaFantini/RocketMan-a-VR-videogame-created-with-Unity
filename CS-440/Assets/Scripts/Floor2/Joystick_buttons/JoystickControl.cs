using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the lever, moves the lever to its final position when colliding with the hand
/// Only works when the previous riddles has been finished
/// </summary>
public class JoystickControl : MonoBehaviour
{

    public bool canMove = false;
    private bool done=false;
    private bool entered = false;
    public ButtonLamp buttonLamp;
    public GameObject sphere;
    public Quaternion finalRotation;

    /// <summary>
    /// If the lever is touch by the hand and the riddles is not finished, moves the lever to its final position
    /// </summary>
    private void Update()
    {
        if(entered && !done)
        {
            StartCoroutine(leverUp(sphere, finalRotation, 1f));
        }
    }

    /// <summary>
    /// Checks on collision if the riddle is not finished and the lever can move
    /// If the lever is touch by the hand, update will start the movement
    /// </summary>
    /// <param name="other">Object colliding</param>
    private void OnTriggerEnter(Collider other) {
        if (canMove && !done && other.gameObject.tag == "PlayerHand" )
        {
            entered = true;
        }
    }

    /// <summary>
    /// Moves the lever to its final position
    /// </summary>
    /// <param name="objectToMove">The lever</param>
    /// <param name="end">End rotation</param>
    /// <param name="speed">Speed</param>
    /// <returns></returns>
    public IEnumerator leverUp(GameObject objectToMove, Quaternion end, float speed)
    {
        while (objectToMove.transform.rotation != end)
        {
            objectToMove.transform.rotation = Quaternion.Slerp(objectToMove.transform.rotation, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            
        }
        buttonLamp.lightColor = ButtonLamp.eColor.Green;
        GameManager.Instance.UpdateGameState(RiddlesProgress.RocketLaunched);
        done = true;
    }
}
