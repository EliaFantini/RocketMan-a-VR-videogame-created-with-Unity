using System;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerController : MonoBehaviour {

	protected List<Type> list_of_player_upgrades = new List<Type>();

	public void acquire_item ( CollectibleItem item ) {
		// Check that the upgrade is not already acquired
		if ( is_equiped_with( item.GetType() ) ) return;

		// Add the upgrade in the list of upgrade collected
		list_of_player_upgrades.Add( item.GetType() );
	}

	public bool is_equiped_with ( Type type ) {
		// Check that one element is the right type
		for ( int i = 0; i < list_of_player_upgrades.Count; i++ ) if ( type == list_of_player_upgrades[i] ) return true;
		return false;
	}


	void OnTriggerEnter ( Collider other ) {

		// Retreive the object to be collected if it exits
		InteractiveItem interactive_item = other.GetComponent<InteractiveItem>();
		if ( interactive_item == null ) return;

		// Forward the current player to the object to be collected
		interactive_item.interacted_with( this );

	}
}