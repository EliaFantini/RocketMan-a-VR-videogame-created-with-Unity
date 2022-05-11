using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallWhenHit : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter (Collider other) {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }
}
