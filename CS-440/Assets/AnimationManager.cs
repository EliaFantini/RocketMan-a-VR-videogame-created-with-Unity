using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    public Animator externalAnim;
    public Animator speedEffectAnim;

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

    public void rocketLaunchAnimation()
    {
        externalAnim.SetBool("rocketLaunched", true);
        speedEffectAnim.gameObject.SetActive(true);
        speedEffectAnim.SetBool("rocketLaunched", true);
    }

}


