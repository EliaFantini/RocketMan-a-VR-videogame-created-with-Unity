
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
           movement.y -= gravity * Time.deltaTime;  //Gravity already implemented in OVRPlayerController
       }

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
