using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWindow : MonoBehaviour {
    public GameObject glassPanel;
    public GameObject wrench;
    public GameObject wrenchNotGrabbable;
    private bool broken = false;

    public void Start()
    {
        //wrench.GetComponent<Rigidbody>().isKinematic = true;
       
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "hammer" && !broken){
            Destroy(glassPanel);
            gameObject.GetComponent<BreakableWindow>().breakWindow();
            broken = true;
            Vector3 grabbablePos = wrenchNotGrabbable.transform.position;
            wrenchNotGrabbable.transform.position = wrench.transform.position;
            wrench.transform.position = grabbablePos;
            wrench.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
            //wrench.GetComponent<Rigidbody>().isKinematic = false;
            wrench.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -5, 0));
        }
    }
}
