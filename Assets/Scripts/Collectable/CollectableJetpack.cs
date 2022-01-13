using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectableJetpack : Collectable
{
    [SerializeField] AK.Wwise.Event OnThrowAKEvent;

    [SerializeField] private int initialCharges;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private TextMeshProUGUI text;
    
    private int curentCharges;

    public int CurentCharges
    {
        get => curentCharges;
        set
        {
            curentCharges = value;
            text.text = curentCharges.ToString();
        }
    }

    public override void OnResetFunction() 
    {
        base.OnResetFunction();

        CurentCharges = initialCharges;

        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D)
            collider2D.enabled = true;
    }

    public override void Interact (CollectableInteraction interactor)
    {
        CurentCharges--;
        Debug.Log($"Interact {CurentCharges}");
        if (interactor.JetpackLaunch())
        {
            OnThrowAKEvent?.Post(gameObject);
        }

        if (CurentCharges < 1)
        {
            particleSystem.transform.parent = null;
            
            particleSystem.Play();
            interactor.RemoveCollectable();
            Destroy(gameObject);
        }
        
    }

    private void Update()
    {
        text.transform.rotation = Quaternion.identity;
    }

    public override void TriggerEvent(Collider2D collision) 
    {
         base.TriggerEvent(collision);
        //
        // Door star = collision.GetComponent<Door>();
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
