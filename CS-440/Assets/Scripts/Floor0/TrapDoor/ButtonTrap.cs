using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum eColor
{
    Red,
    Yellow,
    Green,
    Blue,
}

/// <summary>
/// Class attached to the button used to open the door, allows the button to be enabled and pressed to open the trap door
/// </summary>
public class ButtonTrap : MonoBehaviour
{
    [SerializeField]
    public Vector3 pressedPos;
    public Vector3 unpressedPos;
    public GameObject Door;
    public GameObject button;
    GameObject presser;
    AudioSource sound;
    public bool isPressed;
    public bool enabled = false;
    public ButtonLamp lamp;
    public TrapDoorController trapDoorController;
    public AudioClip unlockedSound;

    void Start()
    {
        sound = button.GetComponent<AudioSource>();
        isPressed = false;
        enabled = false;

        lamp = button.GetComponent<ButtonLamp>();
    }

    /// <summary>
    /// If an object is colliding with the button, open the trap door if the object has the tag "buttonPresser"
    /// </summary>
    /// <param name="other">The object colliding</param>
    private void OnTriggerStay(Collider other) {
        if(!isPressed) {

            //Move the button downward to show that it is pressed
            button.transform.position = pressedPos;
            isPressed = true;

            presser = other.gameObject;
            if (enabled && other.gameObject.tag == "buttonPresser")
            {
                // open the trap
                trapDoorController.setIsOpen(true);
                GameManager.Instance.UpdateGameState(RiddlesProgress.TrapDoorButton);
            }
            else
            {
                // if the object is wrong, play a wrong effect sound
                GetComponent<AudioSource>().Play();
            }
            //button pressed sound
            sound.Play();

        }
    }

    /// <summary>
    /// On trigger exit, close the trap door if it was previously opened (i.e. if the object has the tag "buttonPresser")
    /// </summary>
    /// <param name="other">The object leaving the collider</param>
    private void OnTriggerExit(Collider other) {
        if(isPressed && other.gameObject == presser) {
            //Move the button upward to show that he is unpressed
            button.transform.position = unpressedPos;
            isPressed = false;
            if (enabled && other.gameObject.tag == "buttonPresser")
            {
                // Close the trap
                trapDoorController.setIsOpen(false);
            }
            sound.Play();
        }
    }

    /// <summary>
    /// Enable the button when called and changes its color to green
    /// If the button was already pressed by a button presser (the statue), it opens the trapdoor
    /// </summary>
    public void enableButton()
    {
        enabled = true;
        lamp.on = true;
        lamp.lightColor = ButtonLamp.eColor.Green;
        // Play unlocking sound
        AudioSource.PlayClipAtPoint(unlockedSound, transform.position);

        if(isPressed && presser.tag == "buttonPresser")
        {
            // open the trap
            trapDoorController.setIsOpen(true);
            GameManager.Instance.UpdateGameState(RiddlesProgress.TrapDoorButton);
        }
    }
}

