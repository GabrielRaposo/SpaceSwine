using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{   
    GameObject previousHolder;

    public virtual void Interact (CollectableInteraction interactor) { }

    private void OnTriggerEnter2D (Collider2D collision) 
    {
        TriggerEvent (collision);
    }
    
    public virtual void TriggerEvent(Collider2D collision)
    {
        CollectableInteraction interaction = collision.GetComponent<CollectableInteraction>();
        if (interaction)
        {
            if (collision.gameObject == previousHolder)
                return;

            if (interaction.SetCurrentCollectable (this))
            {
                previousHolder = collision.gameObject; 
            }

            return;
        }
    }
}
