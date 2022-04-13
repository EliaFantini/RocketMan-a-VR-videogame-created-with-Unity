using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateRemoval : MonoBehaviour
{
    [SerializeField]
    public GameObject screw1;
    public GameObject screw2;
    public GameObject screw3;
    public GameObject screw4;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(screw1.GetComponent<Rigidbody>().isKinematic == false
        && screw2.GetComponent<Rigidbody>().isKinematic == false
        && screw3.GetComponent<Rigidbody>().isKinematic == false
        && screw4.GetComponent<Rigidbody>().isKinematic == false)
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
