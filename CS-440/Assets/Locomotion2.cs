using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Locomotion2 : MonoBehaviour
{
	public GameObject hand;

   [Header("Marker")]
	public GameObject markerPrefab;
	[Header("Maximum Distance")]
	[Range(2f, 30f)]
	public float maximumTeleportationDistance = 15f;
	
	// Store the maximum distance the player can teleport
	
	// Store the refence to the marker prefab used to highlight the targeted point
	
	protected bool teleportation_locked = false;
	protected GameObject marker_prefab_instanciated;

	protected CharacterController character_controller;
    void Start() {
		character_controller = this.GetComponent<CharacterController>(); 
	}

	void Update()
	{

		// Store the position of the targeted point
		Vector3 target_point;
		if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp)  && aim_with(out target_point) )
		{

			// Instantiate the marker prefab if it doesn't already exists and place it to the targeted position
			if (marker_prefab_instanciated == null) marker_prefab_instanciated = GameObject.Instantiate(markerPrefab, this.transform);
			marker_prefab_instanciated.transform.position = target_point;

			// Deduce the non pointing hand
			

			// Prevent continuous teleportation
			if (teleportation_locked) return;
			teleportation_locked = true;

			// Tell the character controller to move to the teleportation point
			character_controller.Move(target_point - this.transform.position);


		}
		else
		{
			// Remove the cursor
			if (marker_prefab_instanciated != null) Destroy(marker_prefab_instanciated);
			marker_prefab_instanciated = null;

			// Reset the teleportation state
			teleportation_locked = false;
		}

	}

	protected bool aim_with( out Vector3 target_point)
	{

		// Default the "output" target point to the null vector
		target_point = new Vector3();

		// Launch the ray cast and leave if it doesn't hit anything
		RaycastHit hit;
		if (!Physics.Raycast(hand.transform.position, hand.transform.forward, out hit, Mathf.Infinity)) return false;

		// If the aimed point is out of range (i.e. the raycast distance is above the maximum distance) then prevent the teleportation
		if (hit.distance > maximumTeleportationDistance) return false;

		// "Output" the target point
		target_point = hit.point;
		return true;
	}
}
