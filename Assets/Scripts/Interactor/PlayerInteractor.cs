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

    public void AddInteractable(Interactable npc)
    {
        if (interactableList.Contains(npc))
            return;

        interactableList.Add(npc);
    }

    public void RemoveInteractable(Interactable npc)
    {
        if (!interactableList.Contains(npc))
            return;

        interactableList.Remove(npc);
    }

    public bool Interact()
    {
        // Check if player is idle

        if (!checkGround.OnGround)
            return false;

        if (interactableList.Count < 1)
            return false;

        interactableList[0].Interaction(this);

        return true;
    }
}
