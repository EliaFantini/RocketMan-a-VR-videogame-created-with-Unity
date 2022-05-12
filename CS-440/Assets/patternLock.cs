using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patternLock : MonoBehaviour
{
    [SerializeField]
    public GameObject[] colliders;
    public GameObject box;
    public Material matZlock;
    public Material matlock;


    public bool[] triggers;
    private int timeWindow;
    private int count;
    private bool open;
    private bool checkpoint;
    private GameObject light;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        checkpoint = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(light && light.GetComponent<Light>().enabled)
            gameObject.GetComponent<MeshRenderer>().material = matZlock;

        else if(light && !light.GetComponent<Light>().enabled)
            gameObject.GetComponent<MeshRenderer>().material = matlock;


        for(int i = 0; i < colliders.Length; i ++){
            triggers[i] = colliders[i].GetComponent<screenCollider>().trigger;
        }

        for(int i = 0; i < triggers.Length; i ++){
            if(i == count && triggers[i] == true){
                checkpoint = true;
            }
            else if(checkpoint && i == count && triggers[i] == false){
                count += 1;
                checkpoint = false;
            }
            else if(i != count && triggers[i] == true){
                count =0;
                checkpoint = false;
            }
        }
        if(count == triggers.Length){
            box.GetComponent<Animator>().enabled = true;;
            count = 0;
            checkpoint = false;
        }

    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag =="uvLight")
            light = other.gameObject;
        if(other.GetComponent<Light>().enabled){
            gameObject.GetComponent<MeshRenderer>().material = matZlock;
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if(other.tag == "uvLight"){
            light = null;
            gameObject.GetComponent<MeshRenderer>().material = matlock;
        }
    }
}
