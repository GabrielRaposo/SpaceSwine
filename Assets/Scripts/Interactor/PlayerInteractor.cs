using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(PlatformerCharacter))]
public class PlayerInteractor : MonoBehaviour
{
    PlatformerCharacter platformerCharacter;
    CheckGround checkGround;   
    List <Interactable> interactableList = new List<Interactable>();

    private void Start()
    {
        platformerCharacter = GetComponent<PlatformerCharacter>();
        checkGround = GetComponent<CheckGround>();
    }

    public void AddInteractable(Interactable interactable)
    {
        if (interactableList.Contains(interactable))
            return;

        interactableList.Add (interactable);
    }

    public void RemoveInteractable(Interactable interactable)
    {
        if (!interactableList.Contains(interactable))
            return;

        interactableList.Remove (interactable);
    }

    public bool Interact()
    {
        // Check if player is idle / not stunned / not in cutscene
        if (platformerCharacter.LocalSpeed().y > .5f)
            return false;
        
        if (!checkGround.OnGround) 
            return false;

        if (interactableList.Count < 1)
            return false;

        platformerCharacter.KillInputs();
        interactableList[0].Interaction(this);

        return true;
    }
}
