using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableThrowable : Collectable
{
    [SerializeField] AK.Wwise.Event OnThrowAKEvent;

    public override void OnResetFunction() 
    {
        base.OnResetFunction();

        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D)
            collider2D.enabled = true;
    }

    public override void Interact (CollectableInteraction interactor) 
    {
        if (interactor.LaunchInput())
        {
            OnThrowAKEvent?.Post(gameObject);
        }
    }

    public override void TriggerEvent(Collider2D collision) 
    {
        base.TriggerEvent(collision);

        Star star = collision.GetComponent<Star>();
        if (star)
        {
            star.Collect(this);
            return;
        }
    }
}
