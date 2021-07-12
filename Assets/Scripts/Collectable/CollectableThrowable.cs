using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableThrowable : Collectable
{
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
