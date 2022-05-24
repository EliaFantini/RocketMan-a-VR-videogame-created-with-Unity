using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonSwitch : MonoBehaviour
{

	/*// Define new event type (i.e. we want the function to be called to expect one argument with the boolean type)
	public delegate void SwitchEvent ( bool is_turned_on );

	// Store the reference to the function to call with an argument when an interaction with the player occurs
	protected SwitchEvent on_toggled_callback_with_arg;

	// Allow external objects (such as a controller) to tell which function to call with an argument when an interaction with the player occurs
	public void on_toggled ( SwitchEvent callback ) { on_toggled_callback_with_arg = callback; }

	// Store switch state
	public bool turnedOn = false;

	public override void self_toggled_by (MainPlayerController player ) {

		// Inherit the parent behavior
		base.self_toggled_by(player);

		// Change the switch state
		turnedOn = !turnedOn;

		// Call the referenced of the stored function with the switch state
		if (on_toggled_callback_with_arg != null) on_toggled_callback_with_arg(turnedOn);
	}
}*/
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    GameObject presser;
    AudioSource sound;
    bool isPressed;
    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
    }

    private void OntriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (!isPressed)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            sound.Play();
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0.015f, 0);
            onRelease.Invoke();
            isPressed = false;
        }
    }

    public void SpawnSphere()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        sphere.transform.localPosition = new Vector3(0, 1, 2);
        sphere.AddComponent<Rigidbody>();
    }
}


