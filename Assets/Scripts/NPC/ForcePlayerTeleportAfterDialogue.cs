using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableNPC))]
public class ForcePlayerTeleportAfterDialogue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        interactableNPC.OnDialogueEnd += () => 
        {
            PauseSystem.Instance.GoToShip();
        };
    }
}
