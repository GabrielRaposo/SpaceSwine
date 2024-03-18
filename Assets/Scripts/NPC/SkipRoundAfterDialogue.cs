using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipRoundAfterDialogue : MonoBehaviour
{
    void Start()
    {
        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        RoundsManager roundManager = GetComponentInParent<RoundsManager>();
        interactableNPC.OnDialogueEnd += () => 
        {
            if (!roundManager)
                return;

            roundManager.NextRoundLogic();
        };
    }
}
