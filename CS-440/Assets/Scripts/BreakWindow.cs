using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWindow : MonoBehaviour {
    public GameObject glassPanel;
    public GameObject wrench;
    private bool broken = false;

    public void Start()
    {
        wrench.GetComponent<Rigidbody>().isKinematic = true;
        wrench.GetComponent<OVRGrabbable>().enabled = false;
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "hammer" && !broken){
            Destroy(glassPanel);
            gameObject.GetComponent<BreakableWindow>().breakWindow();
            broken = true;
            wrench.GetComponent<OVRGrabbable>().enabled = true;
            wrench.GetComponent<Rigidbody>().isKinematic = false;
            wrench.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -5, 0));
        }
    }
}
