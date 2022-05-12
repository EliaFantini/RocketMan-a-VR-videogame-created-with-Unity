using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenCollider : MonoBehaviour
{
    public bool trigger=false;
    public screenCollider previousCollider;
    public screenCollider nextCollider;
    public patternLock patternLock;
    

    private void OnTriggerEnter(Collider other) {
        if( previousCollider == null || (previousCollider.trigger == true && nextCollider == null) || (previousCollider.trigger == true && nextCollider.trigger == false))
        {
            trigger = true;
            if (patternLock != null)
            {
                patternLock.onCorrectPattern();
            }
        }
        else
        {
            trigger = false;
            if(previousCollider != null)
            {
                previousCollider.backwardReset();
            }
            if (nextCollider != null)
            {
                nextCollider.forwardReset();
            }
        }
        
    }
    
    public void backwardReset()
    {
        trigger = false;
        if (previousCollider != null)
        {
            previousCollider.backwardReset();
        }
    }

    public void forwardReset()
    {
        trigger = false;
        if (nextCollider != null)
        {
            nextCollider.forwardReset();
        }
    }
}
