using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleDoor : MonoBehaviour
{
    public void open () {
        GetComponent<Animator>().SetBool("correctCode", true); 
	}

}
