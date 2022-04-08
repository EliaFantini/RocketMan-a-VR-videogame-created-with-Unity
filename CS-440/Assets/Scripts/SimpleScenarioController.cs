using UnityEngine;

public class SimpleScenarioController : Controller {

	[Header( "Contolled Items" )]
	public Fence fenceA;
	public Fence fenceB;

	[Header( "Inputs" )]
	public FloorSwitch floorSwitchA;
	public FloorSwitch floorSwitchB;

	void Start () {
		floorSwitchA.on_toggled( on_swich_a_toggled );
		floorSwitchB.on_toggled( on_swich_b_toggled );
	}

	public void on_swich_a_toggled( bool switch_state ) {
		if ( switch_state ) fenceA.open();
		else fenceA.close();
	}

	public void on_swich_b_toggled ( bool switch_state ) {
		if ( switch_state ) fenceB.open();
		else fenceB.close();
	}

}

/**
void Start () {
        floorSwitchA.on_toggled( ( switch_state ) => { if ( switch_state ) fenceA.open(); else fenceA.close();} );
        floorSwitchB.on_toggled( ( switch_state ) => { if ( switch_state ) fenceB.open(); else fenceB.close();} );
}
**/