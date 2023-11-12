using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorAfterDialogue : MonoBehaviour
{
    [SerializeField] Door door;    
    bool used;

    void Start()
    {
        if (door == null)
            return;

        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        interactableNPC.OnDialogueEnd += () => 
        {
            if (used)
                return;

            used = true;
            door.SetOpenState();
        };

        Round round = GetComponentInParent<Round>();
        if (round == null)
            return;

        round.OnReset += () => 
        {
            used = false;
        };
    }

}
