using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] 
    public AudioClip[] riddlesClues;
    public AudioSource mainAudioSource;


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
        if (OVRInput.Get(OVRInput.RawButton.Y))
        {
            int currentRiddleIndex = (int)GameManager.Instance.currentState;
            mainAudioSource.PlayOneShot(riddlesClues[currentRiddleIndex]);
            
        }
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }
}
