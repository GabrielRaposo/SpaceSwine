using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float angularSpeed;
    private Round round;
    private float startingRotation;

    private bool active;
    [SerializeField] private bool startsActive;
    
    private void Start()
    {
        round = GetComponentInParent<Round>();
        startingRotation = transform.eulerAngles.z;

        active = startsActive;
        
        if (round)
            round.OnReset += Reset;
    }

    public void Activate()
    {
        active = true;
    }

    private void FixedUpdate() 
    {
        if(!active)
            return;
        
        transform.Rotate (Vector3.forward * angularSpeed * Time.fixedDeltaTime);    
    }

    public void Reset()
    {
        transform.eulerAngles = new Vector3(0f, 0f, startingRotation);
        active = startsActive;
    }
}
