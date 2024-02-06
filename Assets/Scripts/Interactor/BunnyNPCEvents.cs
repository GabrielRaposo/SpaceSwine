using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyNPCEvents : MonoBehaviour
{
    void Start()
    {
        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        interactableNPC.OnDialogueEnd += () => 
        {
            interactableNPC.SetInteraction(false);
            GetComponentInChildren<Animator>().SetTrigger("Unmask");
        };
    }
}
