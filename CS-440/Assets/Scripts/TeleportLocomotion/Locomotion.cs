using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class which toggles the teleport on the right thumb stick input
/// </summary>
public class Locomotion : MonoBehaviour
{

    public Teleporter teleporter;
    private bool locked = false;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp) && !locked)
        {
            locked = true;
            teleporter.ToggleDisplay(true);        
        }

        if (OVRInput.GetUp(OVRInput.RawButton.RThumbstickUp) && locked)
        {
            locked = false;
            teleporter.ToggleDisplay(false);
            teleporter.Teleport();           
        }
    }
}