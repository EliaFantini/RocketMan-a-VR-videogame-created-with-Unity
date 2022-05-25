using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the wrench, rotate the bolt on contact until the desired angle is found
/// </summary>
public class wrench : MonoBehaviour
{
    private GameObject bolt;
    
    private float prevRot;
    private bool onbolt;
    private Rigidbody rb;
    public bool boltScrewed;
    public TrapDoorController trapDoor;
    [SerializeField]
    public ParticleSystem smoke;

    void Start()
    {
        prevRot = transform.localRotation.eulerAngles.y;
        onbolt = false;
    }

    /// <summary>
    /// If the wrench is on the bolt and the bolt is not screwed, rotate the bolt at the same time as the wrench
    /// </summary>
    void Update()
    {
        if(onbolt && ! boltScrewed){
            float angle = transform.rotation.eulerAngles.y - prevRot;
            rb.transform.Rotate(0,0, angle, Space.Self);
            prevRot = transform.rotation.eulerAngles.y;
        }  
    }

    /// <summary>
    /// On trigger enter, looks if the wrench is on the bolt
    /// </summary>
    /// <param name="other">Other collider</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "bolt")
        {
            bolt = other.gameObject;
            rb = other.GetComponent<Rigidbody>();
            onbolt = true;
        }
    }

    /// <summary>
    /// On trigger exit, looks if the wrench exit the bolt
    /// </summary>
    /// <param name="other">Other collider</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == bolt)
        {
            rb = null;
            onbolt = false;
        }
    }

    /// <summary>
    /// When the bolt is correctly rotated, open the trap door for the third floor
    /// </summary>
    public void boltRotated()
    {
        boltScrewed = true;
        smoke.Play();
        smoke.GetComponent<AudioSource>().Play();
        trapDoor.setIsOpen(true);
        GameManager.Instance.UpdateGameState(RiddlesProgress.TurnOnEngine);
    }
}
