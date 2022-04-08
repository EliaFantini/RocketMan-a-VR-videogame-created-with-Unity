public class PurpleObject : ObjectAnchor {
	public override bool can_be_grasped_by ( MainPlayerController player ) { return player.is_equiped_with( typeof( BasicGraspUpgrade ) ) && player.is_equiped_with( typeof( PurpleUpgrade ) ); }
}