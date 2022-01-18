using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockGravityField : MonoBehaviour
{
    private Collectable capturedCollectable;
    public float pullSpeed;
    public float rotationSpeed;
    
    private Round _round;

    public void GetCollectable(CollectableThrowable collectable)
    {
        capturedCollectable = collectable;
        
        collectable.SetIndestructible(true);
        collectable.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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

        var colectableTransform = capturedCollectable.transform;
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
