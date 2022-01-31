using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [HideInInspector] public bool autoActivate = true;

    protected bool interactable = true;

    public virtual void Interaction (PlayerInteractor interactor) { }
    
    public virtual void SetInteraction (bool value) 
    {
        interactable = value;
    }

    public virtual void IconState(bool value) { }

    protected virtual void HighlightState(bool value) { }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerInteractor playerInteractor = collision.GetComponent<PlayerInteractor>();
        if (!playerInteractor)
            return;

        playerInteractor.AddInteractable(this);
        HighlightState(true);
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerInteractor playerInteractor = collision.GetComponent<PlayerInteractor>();
        if (!playerInteractor)
            return;

        playerInteractor.RemoveInteractable(this);
        HighlightState(false);
    }
}
