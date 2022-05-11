using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenCollider : MonoBehaviour
{
    public bool trigger;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        trigger = true;
    }
    private void OnTriggerExit(Collider other) {
        trigger = false;
    }
}
