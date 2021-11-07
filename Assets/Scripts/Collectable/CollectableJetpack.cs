using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableJetpack : Collectable
{
    [SerializeField] AK.Wwise.Event OnThrowAKEvent;

    [SerializeField] private int initialCharges;
    private int curentCharges;

    public override void OnResetFunction() 
    {
        base.OnResetFunction();

        curentCharges = initialCharges;

        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D)
            collider2D.enabled = true;
    }

    public override void Interact (CollectableInteraction interactor) 
    {
        if (interactor.LaunchInput(false))
        {
            OnThrowAKEvent?.Post(gameObject);
        }
    }

    public override void TriggerEvent(Collider2D collision) 
    {
         base.TriggerEvent(collision);
        //
        // Star star = collision.GetComponent<Star>();
        // if (star)
        // {
        //     star.Collect(this);
        //     return;
        // }
        //
        // Lock l = collision.GetComponent<Lock>();
        // if (l)
        // {
        //     l.Collect(this);
        //     return;
        // }
    }
}
