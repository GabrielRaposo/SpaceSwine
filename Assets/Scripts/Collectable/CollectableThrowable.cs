using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CollectableThrowable : Collectable
{
    [SerializeField] AK.Wwise.Event OnThrowAKEvent;
    [SerializeField] SpriteRenderer visualComponent;
    [SerializeField] GameObject destroyParticles;

    private bool indestructible;

    public override void OnResetFunction() 
    {
        base.OnResetFunction();

        indestructible = false;
        
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

    public void SetIndestructible(bool value)
    {
        indestructible = value;
    }

    public override void TriggerEvent(Collider2D collision) 
    {
        Lock l = collision.GetComponent<Lock>();
        if (l)
        {
            l.Collect(this);
            return;
        }
        
        if(indestructible) return;
        
        base.TriggerEvent(collision);

        LockGravityField lgf = collision.GetComponent<LockGravityField>();
        if (lgf)
        {
            lgf.GetCollectable(this);
        }

        Hitbox hb = collision.GetComponent<Hitbox>();
        if (hb)
        {
            if (hb.damage > 0)
            {
                Debug.Log($"{gameObject.name} <color=#dd0000><b>Take damage</b></color>");
                DestroyKey();
            }
                
        }
    }

    private void DestroyKey()
    {
        gameObject.SetActive(false);
        Instantiate(destroyParticles, transform.position, quaternion.identity);

    }
}
