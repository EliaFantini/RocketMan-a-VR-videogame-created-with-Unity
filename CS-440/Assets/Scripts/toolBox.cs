using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toolBox : MonoBehaviour
{

    [SerializeField]
    public GameObject[] boxTools;
    public Vector3 boxBoundaries;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject tool in boxTools)
        {
            if(tool.transform.parent != null && (Mathf.Abs(tool.transform.position.y- gameObject.transform.position.y) > boxBoundaries.y
            || Mathf.Abs(tool.transform.position.x- gameObject.transform.position.x) > boxBoundaries.x
            || Mathf.Abs(tool.transform.position.z- gameObject.transform.position.z) > boxBoundaries.z)){
                tool.transform.parent = null;
            }
        }
    }
}
