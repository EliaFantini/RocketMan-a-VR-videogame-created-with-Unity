public class FloorSwitch : Switch {

	// Define new event type (i.e. we want the function to be called to expect one argument with the boolean type)
	public delegate void SwitchEvent ( bool is_turned_on );

	// Store the reference to the function to call with an argument when an interaction with the player occurs
	protected SwitchEvent on_toggled_callback_with_arg;

	// Allow external objects (such as a controller) to tell which function to call with an argument when an interaction with the player occurs
	public void on_toggled ( SwitchEvent callback ) { on_toggled_callback_with_arg = callback; }


	// Store switch state
	public bool turnedOn = false;

	public override void self_toggled_by ( MainPlayerController player ) {

		// Inherit the parent behavior
		base.self_toggled_by( player );

		// Change the switch state
		turnedOn = !turnedOn;

		// Call the referenced of the stored function with the switch state
		if ( on_toggled_callback_with_arg != null ) on_toggled_callback_with_arg( turnedOn );
	}
}