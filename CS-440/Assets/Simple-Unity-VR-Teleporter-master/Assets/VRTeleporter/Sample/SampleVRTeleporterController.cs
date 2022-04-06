using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleVRTeleporterController : MonoBehaviour {

    public Teleporter teleporter;

	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            teleporter.ToggleDisplay(true);
        }

        if(Input.GetMouseButtonUp(0))
        {
            teleporter.Teleport();
            teleporter.ToggleDisplay(false);
        }
	}
}
