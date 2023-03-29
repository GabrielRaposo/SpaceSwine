using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{   
    [Header("Wwise Events")]
    [SerializeField] AK.Wwise.Event OnCollectAKEvent;

    GameObject previousHolder;
    ConcealingBody concealingBody;

    private void Start() 
    {
        Round round = GetComponentInParent<Round>();
        if (round)
        {
            round.OnReset += OnResetFunction;
        }
    }

    public virtual void OnResetFunction()
    {
        gameObject.SetActive(true);
        SetInteractable(true);
        UpdateSortingLayer(true);
        previousHolder = null;
    }

    public virtual void Interact (CollectableInteraction interactor) { }

    private void OnTriggerEnter2D (Collider2D collision) 
    {
        TriggerEvent (collision);
    }
    
    public virtual void TriggerEvent(Collider2D collision)
    {
        if (concealingBody != null && concealingBody.IsActive())
            return;

        CollectableInteraction interaction = collision.GetComponent<CollectableInteraction>();
        if (interaction)
        {
            if (collision.gameObject == previousHolder)
                return;

            if (interaction.SetCurrentCollectable (this))
            {
                OnCollected();

                previousHolder = collision.gameObject; 
            }

            return;
        }
    }

    public virtual void OnCollected()
    {
        OnCollectAKEvent?.Post(gameObject);
    }

    public virtual void SetInteractable(bool value)
    {
        Collider2D coll = GetComponent<Collider2D>();
        if (coll)
            coll.enabled = value;
    }

    public virtual void UpdateSortingLayer(bool value)
    {
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer)
            renderer.sortingLayerName = (value ? "Item" : "Default");
    }

    public void NullifyPreviousHolder()
    {
        previousHolder = null;
    }

    public void SetConcealingBody (ConcealingBody concealingBody)
    {
        this.concealingBody = concealingBody;
    }
}
