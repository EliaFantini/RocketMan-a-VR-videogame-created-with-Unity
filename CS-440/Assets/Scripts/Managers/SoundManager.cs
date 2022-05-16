using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] 
    public AudioClip[] riddlesClues;
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

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {
            int currentRiddleIndex = (int)GameManager.Instance.currentState;
            mainAudioSource.PlayOneShot(riddlesClues[currentRiddleIndex], 3f);
            
        }
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            backgroundMusic.mute = !backgroundMusic.mute;
        }
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

}
