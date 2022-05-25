using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the correct code has been entered, open the capsule door
/// </summary>
public class CapsuleDoor : MonoBehaviour
{
    public void open () {
        GetComponent<Animator>().SetBool("correctCode", true);
        GameManager.Instance.UpdateGameState(RiddlesProgress.DoorCodeInserted);
	}
}
