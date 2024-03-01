using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyNPCEvents : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event unmaskAKEvent;

    void Start()
    {
        InteractableNPC interactableNPC = GetComponent<InteractableNPC>();
        interactableNPC.OnDialogueEnd += () => 
        {
            interactableNPC.SetInteraction(false);
            GetComponentInChildren<Animator>().SetTrigger("Unmask");

            if (unmaskAKEvent != null)
                unmaskAKEvent.Post(gameObject);
        };
    }
}
