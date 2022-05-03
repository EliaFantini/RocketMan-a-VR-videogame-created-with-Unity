using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wrench : MonoBehaviour
{

    private Vector3 prevPos;
    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<Rigidbody>().isKinematic){
            if(transform.position != prevPos)
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
            prevPos = transform.position;
        }
    }
}
