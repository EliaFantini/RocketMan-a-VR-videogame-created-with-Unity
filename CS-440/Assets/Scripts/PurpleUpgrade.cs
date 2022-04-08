public class PurpleUpgrade : Upgrade {

	public override void self_collect_by ( MainPlayerController player ) {

		// Prevent the user to collect the upgrade if he doesn't own the basic grasp
		if ( !player.is_equiped_with( typeof( BasicGraspUpgrade ) ) ) return;

		// Acquire the object through the default behavior otherwise
		base.self_collect_by( player );
	}

}