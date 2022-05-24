using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveItem : MonoBehaviour
{
    public abstract void interacted_with (MainPlayerController player);
    
}
