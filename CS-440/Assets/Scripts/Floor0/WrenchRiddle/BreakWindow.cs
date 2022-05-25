using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Break window on collsiion with the hammer
/// Unfreezes the wrench and removes it from its slot
/// </summary>
public class BreakWindow : MonoBehaviour {
    public GameObject glassPanel;
    public GameObject wrench;
    public GameObject wrenchNotGrabbable;
    private bool broken = false;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "hammer" && !broken){
            GameManager.Instance.UpdateGameState(RiddlesProgress.BrakeWindow);
            Destroy(glassPanel);
            gameObject.GetComponent<BreakableWindow>().breakWindow();
            broken = true;
            Vector3 grabbablePos = wrenchNotGrabbable.transform.position;
            wrenchNotGrabbable.transform.position = wrench.transform.position;
            wrench.transform.position = grabbablePos;
            wrench.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
            wrench.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -5, 0));
            other.gameObject.GetComponentInChildren<OVRGrabbable>().grabbedBy.hapticPulse();
            
        }
    }
}
