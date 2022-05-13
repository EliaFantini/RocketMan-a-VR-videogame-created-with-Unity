using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patternLock : MonoBehaviour
{
    [SerializeField]
    public GameObject[] objectsNotGrabbable;
    public GameObject[] objectsGrabbable;
    public Material matZlock;
    public Material matlock;



    private int timeWindow;

    private bool open;

    private GameObject light;
    // Start is called before the first frame update



    public void onCorrectPattern()
    {
        for (int i = 0; i < objectsGrabbable.Length; i++)
        {
            Vector3 grabbablePos = objectsNotGrabbable[i].transform.position;
            objectsNotGrabbable[i].transform.position = objectsGrabbable[i].transform.position;
            objectsGrabbable[i].transform.position = grabbablePos;
        }
        for (int i = 0; i < objectsGrabbable.Length; i++)
        {
            objectsGrabbable[i].GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "uvLight")
        {
            light = other.gameObject;
            if (other.GetComponent<Light>().enabled)
            {
                gameObject.GetComponent<MeshRenderer>().material = matZlock;
            }

        }
            
    }
    
    private void OnTriggerExit(Collider other) {
        if(other.tag == "uvLight"){
            light = null;
            gameObject.GetComponent<MeshRenderer>().material = matlock;
        }
    }
}
