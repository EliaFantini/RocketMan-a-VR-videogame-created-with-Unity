using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wrench : MonoBehaviour
{


    private GameObject bolt;
    public float totAngle = 0;
    private float prevRot;
    private bool onbolt;
    private Rigidbody rb;
    public bool boltScrewed;
    [SerializeField]
    public ParticleSystem smoke;
    // Start is called before the first frame update
    void Start()
    {
    
        prevRot = transform.localRotation.eulerAngles.y;
        onbolt = false;
    }

    // Update is called once per frame
    void Update()
    {
        


        if(onbolt && ! boltScrewed){
            float angle = transform.rotation.eulerAngles.y - prevRot;
            rb.transform.Rotate(0,0, angle, Space.Self);
            prevRot = transform.rotation.eulerAngles.y;
            totAngle = transform.rotation.y;
        }
        
        if(totAngle >= 0.975 && totAngle <=0.998 && ! boltScrewed && onbolt){
            boltScrewed = true;
            smoke.Play();
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
