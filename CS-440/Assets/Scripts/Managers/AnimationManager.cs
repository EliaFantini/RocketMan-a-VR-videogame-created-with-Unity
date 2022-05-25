using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation manager
/// </summary>
public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    public Animator externalAnim;
    public Animator speedEffectAnim;
    public Animator speedEffectCapsuleAnim;
    public Animator planetAnim;
    public GameObject wall;
    public Vector3 wallFinalPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Start the rocket launching animation
    /// </summary>
    public void rocketLaunchAnimation()
    {
        externalAnim.SetBool("rocketLaunched", true);
        speedEffectAnim.gameObject.SetActive(true);
        speedEffectAnim.SetBool("rocketLaunched", true);
    }

    /// <summary>
    /// Start the capsule launching animation
    /// </summary>
    public void capsuleLaunchAnimation()
    {
        
        speedEffectCapsuleAnim.gameObject.SetActive(true);
        speedEffectCapsuleAnim.SetBool("capsuleLaunched", true);
        planetAnim.SetBool("capsuleLaunched", true);
    }


    /// <summary>
    /// Move the wall of the capsule
    /// </summary>
    public void moveCapsuleWall()
    {
        wall.transform.position = wallFinalPos;
    }

}


