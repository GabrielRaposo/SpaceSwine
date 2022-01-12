using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockGravityField : MonoBehaviour
{
    private Collectable capturedCollectable;
    public float pullSpeed;
    
    private Round _round;

    public void GetCollectalbe(Collectable collectable)
    {
        capturedCollectable = collectable;
    }
    
    private void Start()
    {
        _round = GetComponentInParent<Round>();

        _round.OnReset += Reset;
    }

    private void Reset()
    {
        capturedCollectable = null;
    }


    private void Update()
    {
        if(!capturedCollectable) return;

        var colectablePos = capturedCollectable.transform.position;
        var direction = transform.position - colectablePos;
        
        direction = direction.normalized;
        
        colectablePos = colectablePos + direction*pullSpeed;
        capturedCollectable.transform.position = colectablePos;
    }
}
