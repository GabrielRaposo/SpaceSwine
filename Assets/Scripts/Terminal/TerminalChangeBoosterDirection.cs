using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalChangeBoosterDirection : MonoBehaviour, ITerminalEvent
{
    [SerializeField] List<SpaceBooster> boosters;
    [SerializeField] float duration;
    
    List<Interactable> interactableChildren; 

    void Start()
    {
        interactableChildren = new List<Interactable>();
        GetComponentsInChildren(interactableChildren);
    }

    public void Activate (InteractableTerminal terminal, PlayerInteractor player)
    {
        BeforeSequence(player);

        if (boosters.Count < 1)
        {
            enabled = false;
            return;
        }

        for (int i = 0; i < boosters.Count; i++)
        {
            SpaceBooster booster = boosters[i];

            int localI = i;
            if (localI  == 0)
            { 
                booster.ChangeLaunchDirection
                (
                    duration: duration,
                    afterSequenceAction: () => AfterSequence(player)
                );
            }
            else
            { 
                booster.ChangeLaunchDirection
                (
                    duration: duration,
                    afterSequenceAction: () => {}
                );
            }
        }
    }

    private void BeforeSequence(PlayerInteractor player)
    {
        if (interactableChildren != null && interactableChildren.Count > 0)
        {
            foreach (Interactable i in interactableChildren)
                i.SetInteraction(false);
        }

        if (player)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = false;

            PlatformerCharacter platformer = player.GetComponent<PlatformerCharacter>();
            if (platformer) platformer.KillInputs();
        }
    }

    private void AfterSequence(PlayerInteractor player)
    {
        if (interactableChildren != null && interactableChildren.Count > 0)
        {
            foreach (Interactable i in interactableChildren)
                i.SetInteraction(true);
        }

        if (player)
        {
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = true;
        }
    }

}
