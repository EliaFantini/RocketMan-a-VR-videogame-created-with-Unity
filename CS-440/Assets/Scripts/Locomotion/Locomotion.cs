using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{

    public Teleporter teleporter;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp))
        {
            //teleporter.updateOVRPlayerPos();
            teleporter.ToggleDisplay(true);
        }

        if (OVRInput.GetUp(OVRInput.RawButton.RThumbstickUp))
        {
            
            teleporter.Teleport();
            teleporter.ToggleDisplay(false);
        }
    }
}