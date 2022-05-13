using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UVLight : MonoBehaviour
{
    [SerializeField]
    public GameObject light;
    private bool on;
    private bool firstTimeGrabbed = true;
    // Start is called before the first frame update
    void Start()
    {
        light.GetComponent<Light>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<OVRGrabbable>().isGrabbed){
            if (firstTimeGrabbed)
            {
                firstTimeGrabbed = false;
                GameManager.Instance.UpdateGameState(RiddlesProgress.UVLightGrabbed);
            }
            if (OVRInput.GetDown(OVRInput.Button.One))
                light.GetComponent<Light>().enabled = !light.GetComponent<Light>().enabled;
        }
    }
}
