
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : MonoBehaviour
{
    public float gravity = 45.0f;
    public float sensitivity = 45.0f;
    private Hand currentHand = null; //pulling hand 
    private CharacterController controller = null; //character

    public void Awake() {
        controller = GetComponent<CharacterController>();

    }

    private void Update() {
        CalculateMovement();
    }

    private void CalculateMovement() {
       Vector3 movement = Vector3.zero;

       if(currentHand) {
           movement += currentHand.Delta * sensitivity; //this should work for y axis... why cant i move up ?? 
       } 
       if(movement == Vector3.zero) {
           //movement.y -= gravity * Time.deltaTime;  //Gravity already implemented in OVRPlayerController
       }

       //movement.y += 10f; If you uncomment this you will see that the player "shoots up and down" so basically
       //we can move the player up from here but somewhere else in the code he is pulled back down.
       //mmh maybe add a collision around the stairs, so when im close to stairs update like this....
       //kinda stupid to have OVRPlayercontroller anyway... cuz we dont move any other way..
       //maybe just add teleport to the camera rig
       controller.Move(movement * Time.deltaTime);
    }

    public void SetHand(Hand hand) {
        //release old hand when grab with new hand (ex: release left when holding with right)
        if(currentHand)
            currentHand.ReleasePoint();

        currentHand = hand;
    }

    public void ClearHand() {
        currentHand = null;
    }

}
