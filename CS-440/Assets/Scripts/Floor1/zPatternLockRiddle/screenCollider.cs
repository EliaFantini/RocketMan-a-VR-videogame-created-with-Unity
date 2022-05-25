using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the colliders used to detect the Z drawing
/// </summary>
public class screenCollider : MonoBehaviour
{
    public bool trigger=false;
    private bool alreadyDone = false;
    public screenCollider previousCollider;
    public screenCollider nextCollider;
    public patternLock patternLock;
    
    /// <summary>
    /// On trigger enter, check if this collider was hit before the nextCollider and after the previousCollider using the public bool trigger
    /// If the pattern was completly drawn, it calls on patternLock to make the toolbox fall
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if( previousCollider == null || (previousCollider.trigger == true && nextCollider == null) || (previousCollider.trigger == true && nextCollider.trigger == false))
        {
            trigger = true;
            if (patternLock != null && !alreadyDone)
            {
                patternLock.onCorrectPattern();
                alreadyDone = true;
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
    /// <summary>
    /// Reset trigger when called, then reset the previous collider trigger
    /// </summary>
    public void backwardReset()
    {
        trigger = false;
        if (previousCollider != null)
        {
            previousCollider.backwardReset();
        }
    }

    /// <summary>
    /// Reset trigger when called, then reset the next collider trigger
    /// </summary>
    public void forwardReset()
    {
        trigger = false;
        if (nextCollider != null)
        {
            nextCollider.forwardReset();
        }
    }
}
