﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CupStack : MonoBehaviour
{
    [SerializeField] private List<Rigidbody2D> cups;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!col.gameObject.CompareTag("Player")) return;

        float angle = Mathg.AngleOfTheLineBetweenTwoPoints(transform.position, col.transform.position);
        Vector2 kickDirection;
        
        if(angle<0)
         kickDirection = Mathg.AngleToDirection2(angle).normalized;
        else
            kickDirection = Mathg.AngleToDirection2(angle-180f).normalized;
        
        Debug.Log($"kickDirection: {kickDirection}\nangle: {angle}");
        
        foreach (var rb in cups)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0.001f;
            rb.drag = 0.7f;
            rb.angularDrag = 0.1f;
            var randomDir = angle > 0 ? Random.Range(180f, 360f) : Random.Range(0f, 180f);
            rb.AddForce(kickDirection*2f + Mathg.AngleToDirection2(randomDir)*.5f, ForceMode2D.Impulse);
            //rb.AddForce(kickDirection*2f + Mathg.AngleToDirection2(Random.Range(0f,180f))*0f, ForceMode2D.Impulse);
            float side = Random.Range(0, 2) == 0 ? -1f : 1f;
            rb.AddTorque(Random.Range(20f,50f)*side);
        }

        GetComponent<Collider2D>().enabled = false;

    }
}
