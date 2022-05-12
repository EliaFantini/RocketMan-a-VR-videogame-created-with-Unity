using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wrench : MonoBehaviour
{

    private Vector3 prevPos;
    private GameObject bolt;
    private float totAngle;
    private float prevRot;
    private bool onbolt;
    private Rigidbody rb;
    public bool boltScrewed;
    [SerializeField]
    public float threshold;
    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;
        totAngle = 0;
        prevRot = transform.localRotation.eulerAngles.y;
        onbolt = false;
    }

    // Update is called once per frame
    void Update()
    {
        


        if(onbolt && totAngle < threshold && ! boltScrewed){
            float angle = transform.localRotation.eulerAngles.y - prevRot;
            rb.transform.Rotate(0,0, angle, Space.Self);
            prevRot = transform.localRotation.eulerAngles.y;
            totAngle += angle;
        }

        if(totAngle >= threshold && ! boltScrewed){
            boltScrewed = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "bolt")
        {
            bolt = other.gameObject;
            rb = other.GetComponent<Rigidbody>();
            onbolt = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == bolt)
        {
            rb = null;
            onbolt = false;
        }
    }
}
