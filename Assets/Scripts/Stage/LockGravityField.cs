using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockGravityField : MonoBehaviour
{
    public float pullSpeed;
    public float rotationSpeed;

    public Collectable CapturedCollectable { get; private set; }
    
    private Round _round;

    public bool GetCollectable(CollectableThrowable collectable)
    {
        if (CapturedCollectable != null)
            return false;

        CapturedCollectable = collectable;
        
        collectable.SetIndestructible(true);
        collectable.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        return true;
    }
    
    private void Start()
    {
        _round = GetComponentInParent<Round>();

        _round.OnReset += Reset;
    }

    private void Reset()
    {
        CapturedCollectable = null;
    }


    private void FixedUpdate()
    {
        if(!CapturedCollectable) return;

        var colectableTransform = CapturedCollectable.transform;
        var colectablePos = colectableTransform.position;
        var direction = transform.position - colectablePos;
        
        direction = direction.normalized;
        
        colectablePos = colectablePos + direction*pullSpeed;
        colectableTransform.position = colectablePos;
        
        colectableTransform.RotateAround(transform.position, new Vector3(0,0,1f), rotationSpeed);
    }

    private void LateUpdate()
    {
        
    }
}
