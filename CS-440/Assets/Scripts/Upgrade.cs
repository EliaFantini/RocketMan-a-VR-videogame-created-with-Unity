using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Upgrade : CollectibleItem {

        public override void self_collect_by ( MainPlayerController player ) {
		// Trigger the action on the player : Tell the player to acquire the blue grasp
		player.acquire_item( this );

		// As the object is acquired we can now apply the common behavior of collectible items : Call its destruction
		base.self_collect_by( player );
	}

}