﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScreensOverlay : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event turnOnAKEvent;

    Animator animator;

    private void Awake() 
    {
        animator = GetComponentInChildren<Animator>();    
    }

    public void TurnOn ()
    {
        if (!animator)
            return;

        animator.SetTrigger("TurnOn");
    }

    public void Break ()
    {
        if (!animator)
            return;

        animator.SetTrigger("Break");
    }

    public void PlayTurnOnSound()
    {
        if (turnOnAKEvent != null)
            turnOnAKEvent.Post(gameObject);
    }
}