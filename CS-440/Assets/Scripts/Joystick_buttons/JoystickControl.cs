using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickControl : MonoBehaviour
{

    public bool canMove = false;
    private bool done=false;
    private bool entered = false;
    public ButtonLamp buttonLamp;
    public GameObject sphere;
    public Quaternion finalRotation;

    private void Update()
    {
        if(entered && !done)
        {
            StartCoroutine(leverUp(sphere, finalRotation, 1f));
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (canMove && !done && other.gameObject.tag == "PlayerHand" )
        {
            entered = true;
        }
        
    }

    public IEnumerator leverUp(GameObject objectToMove, Quaternion end, float speed)
    {
        while (objectToMove.transform.rotation != end)
        {
            objectToMove.transform.rotation = Quaternion.Slerp(objectToMove.transform.rotation, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            
        }
        buttonLamp.lightColor = ButtonLamp.eColor.Green;
        done = true;
    }
}
