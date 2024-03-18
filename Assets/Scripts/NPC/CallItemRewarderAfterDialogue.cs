using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallItemRewarderAfterDialogue : MonoBehaviour
{
    [SerializeField] ItemRewarder itemRewarder;
    [SerializeField] float delay = 1.5f;

    void Start()
    {
        if (!itemRewarder)
            return;

        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        if (!interactableNPC)
            return;

        interactableNPC.AfterDialogueEnd += () => StartCoroutine (Call (interactableNPC));
    }

    private IEnumerator Call (InteractableNPC interactableNPC)
    {
        PlayerInteractor playerInteractor = interactableNPC.PlayerInteractor;
        if (playerInteractor == null)
            yield break;

        if (delay > 0)
        {
            DialogueSystem.OnDialogue = true;
            DialogueSystem.BlockInputs = true;
            yield return new WaitForSeconds(delay);
            DialogueSystem.OnDialogue = false;
            DialogueSystem.BlockInputs = false;
        }

        itemRewarder.Call (playerInteractor);
    }
}
