using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{

    public Teleporter teleporter;
    private bool locked = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp) && !locked)
        {
            locked = true;
            //teleporter.updateOVRPlayerPos();
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