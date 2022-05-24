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

    private void Update()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        if(!isPressed) {

            //button.transform.position = pressedPos;
            //StartCoroutine(MoveOverSpeed(button, pressedPos, 0.1f, true));
            button.transform.position = pressedPos;
            isPressed = true;

            presser = other.gameObject;
            if (enabled && other.gameObject.tag == "buttonPresser")
            {
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

    private void OnTriggerExit(Collider other) {
        if(isPressed && other.gameObject == presser) {

            //button.transform.position = unpressedPos;
            //StartCoroutine(MoveOverSpeed(button, unpressedPos, 0.1f, false));
            button.transform.position = unpressedPos;
            isPressed = false;
            if (enabled && other.gameObject.tag == "buttonPresser")
            {
                trapDoorController.setIsOpen(false);
            }
            sound.Play();

        }
    }

    public void enableButton()
    {
        enabled = true;
        lamp.on = true;
        lamp.lightColor = ButtonLamp.eColor.Green;
        AudioSource.PlayClipAtPoint(unlockedSound, transform.position);
        if(isPressed && presser.tag == "buttonPresser")
        {
            trapDoorController.setIsOpen(true);
            GameManager.Instance.UpdateGameState(RiddlesProgress.TrapDoorButton);
        }

    }

    
    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed, bool isPressedd)
    {
        /*
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        
           
        }

        */
        
        button.transform.position = end;
        yield return new WaitForSeconds(0.5f);
        isPressed = isPressedd;

    }
    
}

