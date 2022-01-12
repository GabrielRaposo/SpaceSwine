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

        // Door door = collision.GetComponent<Door>();
        // if (door)
        // {
        //     door.Collect(this);
        //     return;
        // }

        LockGravityField lgf = collision.GetComponent<LockGravityField>();
        if (lgf)
        {
            lgf.GetCollectalbe(this);
        }
        
        Lock l = collision.GetComponent<Lock>();
        if (l)
        {
            l.Collect(this);
            return;
        }

        Hitbox hb = collision.GetComponent<Hitbox>();
        if (hb)
        {
            if (hb.damage > 0)
                gameObject.SetActive(false);
    
        }
    }
}
