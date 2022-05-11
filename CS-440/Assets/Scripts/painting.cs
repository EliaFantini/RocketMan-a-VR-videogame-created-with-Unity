using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class painting : MonoBehaviour
{
    [SerializeField]
    public Vector3 truePlace;
    public Vector3 trueRotation;
    public GameObject slidingWall;
    public float threshold;
    public Vector3 slidingDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(truePlace, transform.position) < threshold){
            transform.position = truePlace;
            transform.Rotate(trueRotation, Space.World);
            slidingWall.transform.position =slidingWall.transform.position + slidingDistance;
        }
    }
}
