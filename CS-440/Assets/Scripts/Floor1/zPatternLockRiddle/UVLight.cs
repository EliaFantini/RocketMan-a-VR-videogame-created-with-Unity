using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Update game state when the light is grabbed for the first time
/// </summary>
public class UVLight : MonoBehaviour
{
    [SerializeField]
    public GameObject light;
    private bool firstTimeGrabbed = true;
    private OVRGrabber grabber;
    public AudioSource audioSource;

    void Start()
    {
        light.GetComponent<Light>().enabled = true;
    }

    void Update()
    {   
        if (gameObject.GetComponent<OVRGrabbable>().isGrabbed){
            if (firstTimeGrabbed)
            {
                firstTimeGrabbed = false;
                GameManager.Instance.UpdateGameState(RiddlesProgress.UVLightGrabbed);
            }
        }
    }
}
