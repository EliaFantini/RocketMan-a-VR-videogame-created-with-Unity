using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sound manager
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] 
    public AudioClip[] riddlesClues;
    public AudioClip countdownClip;
    public AudioClip rocketClip;
    public AudioClip capsuleClip;
    public AudioSource mainAudioSource;
    public AudioSource backgroundMusic;


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
    /// Play the hint for the riddles on trigger of Start button, mute music on trigger of the Y button
    /// </summary>
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {
            int currentRiddleIndex = (int)GameManager.Instance.currentState;
            mainAudioSource.PlayOneShot(riddlesClues[currentRiddleIndex], 8f);
            
        }
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            backgroundMusic.mute = !backgroundMusic.mute;
        }
    }

    /// <summary>
    /// Play the countdown for the rocket launch
    /// </summary>
    public void playCountdownClip()
    {
        mainAudioSource.PlayOneShot(countdownClip, 0.2f);
    }

    /// <summary>
    /// Play the rocket clip
    /// </summary>
    public void playRocketClip()
    {
        mainAudioSource.PlayOneShot(rocketClip, 0.2f);
        OVRHapticsClip takeOffClip = new OVRHapticsClip(rocketClip);
        OVRHaptics.LeftChannel.Preempt(takeOffClip);
        OVRHaptics.RightChannel.Preempt(takeOffClip);
    }

    /// <summary>
    /// Play the capsule launching clip
    /// </summary>
    public void playCapsuleClip()
    {
        mainAudioSource.PlayOneShot(capsuleClip, 0.7f);
        OVRHapticsClip takeOffClip = new OVRHapticsClip(capsuleClip);
        OVRHaptics.LeftChannel.Preempt(takeOffClip);
        OVRHaptics.RightChannel.Preempt(takeOffClip);
    }

    /// <summary>
    /// Change value
    /// </summary>
    /// <param name="value">Value of the volume</param>
    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

}
