using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableThrowable : Collectable
{
    public override void OnResetFunction() 
    {
        base.OnResetFunction();

        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D)
            collider2D.enabled = true;
    }

    public override void Interact (CollectableInteraction interactor) 
    {
        interactor.LaunchInput();
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
