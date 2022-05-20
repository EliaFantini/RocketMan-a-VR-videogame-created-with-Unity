using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTabController : MonoBehaviour
{
    
    // Start is called before the first frame update
    private GameObject screen1;
    private GameObject screen2;
    private GameObject screen3;
    private GameObject screen4;
    void Start()
    {
        screen1 = transform.GetChild(0).gameObject;
        screen1.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
