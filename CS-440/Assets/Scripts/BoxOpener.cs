using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOpener : MonoBehaviour
{
    public GameObject box;
    private Animator animator;
    private int startTime;
    private bool breakBox;
    [SerializeField]
    public float breakingLimit;
    public Collider boxTop;
    public GameObject[] tools;
    // Start is called before the first frame update
    void Start()
    {
        animator = box.GetComponent<Animator>();
        animator.enabled = false;
        breakBox = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(breakBox && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f){
          animator.enabled = false;
        }
        if(box.GetComponent<Rigidbody>().velocity.magnitude > breakingLimit)
            breakBox = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(breakBox){
            animator.enabled = true;
            boxTop.enabled = false;
            foreach(GameObject tool in tools)
            {
                tool.GetComponent<OVRGrabbable>().enabled = true;
            }
        }
    }
}
