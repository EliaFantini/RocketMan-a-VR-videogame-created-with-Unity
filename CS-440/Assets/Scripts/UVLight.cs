using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UVLight : MonoBehaviour
{
    [SerializeField]
    public GameObject light;
    private bool on;
    // Start is called before the first frame update
    void Start()
    {
        light.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<OVRGrabbable>().isGrabbed){
            if (OVRInput.GetDown(OVRInput.Button.One))
                light.SetActive(!light.activeSelf);
        }
    }
}
