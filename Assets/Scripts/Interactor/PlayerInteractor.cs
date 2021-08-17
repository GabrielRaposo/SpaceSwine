using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CheckGround))]
public class PlayerInteractor : MonoBehaviour
{
    CheckGround checkGround;   
    List <Interactable> interactableList = new List<Interactable>();

    private void Start()
    {
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

        if (!checkGround.OnGround)
            return false;

        if (interactableList.Count < 1)
            return false;

        interactableList[0].Interaction(this);
        // Change player states

        return true;
    }
}
