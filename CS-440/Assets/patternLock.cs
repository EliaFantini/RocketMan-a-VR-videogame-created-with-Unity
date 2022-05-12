using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patternLock : MonoBehaviour
{
    [SerializeField]
    public GameObject box;
    public Material matZlock;
    public Material matlock;



    private int timeWindow;

    private bool open;

    private GameObject light;
    // Start is called before the first frame update



    public void onCorrectPattern()
    {
        box.GetComponent<Animator>().enabled = true; ;

        
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
