using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressChallengeController : Controller {


	[Header( "Contolled Items" )]
	[Header( "Contolled Items" )]
	public Fence fence;
	//public Fence fenceB;

	[Header( "Inputs" )]
	public ButtonSwitch buttonSwitch1;
	//public ButtonSwitch floorSwitchB;

	void Start () {
		//buttonSwitch1.on_toggled( on_swich_a_toggled );
		//floorSwitchB.on_toggled( on_swich_b_toggled );
	}

	public void on_swich_a_toggled( bool switch_state ) {
		if ( switch_state ) fence.open();
		else fence.close();
	}

	/*public void on_swich_b_toggled ( bool switch_state ) {
		if ( switch_state ) fenceB.open();
		else fenceB.close();
	}*/



}