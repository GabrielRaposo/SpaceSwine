using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{   
    public virtual void Interact (CollectableInteraction interactor) { }
    
    private void OnTriggerEnter2D (Collider2D collision) 
    {
        CollectableInteraction interaction = collision.GetComponent<CollectableInteraction>();
        if (!interaction)
            return;

        interaction.SetCurrentCollectable (this);
    }
}
