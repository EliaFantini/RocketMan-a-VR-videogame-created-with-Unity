using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWindow : MonoBehaviour {


    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "hammer"){
            gameObject.GetComponent<BreakableWindow>().breakWindow();
        }
    }
}
