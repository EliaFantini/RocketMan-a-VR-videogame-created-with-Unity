using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateRemoval : MonoBehaviour
{
    [SerializeField]
    public GameObject screw1;
    public GameObject screw2;

    // Update is called once per frame
    void Update()
    {
        if(screw1.GetComponent<Rigidbody>().isKinematic == false
        && screw2.GetComponent<Rigidbody>().isKinematic == false)
            gameObject.GetComponent<Animator>().SetBool("button_pressed", true);
    }
}
