﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 offset; 
    [SerializeField] Vector2 size; 
    
    bool onGround;
    PlanetPlatform planetPlatform;

    Color debugCollisionColor = Color.blue;

    public bool OnGround
    {
        get { return onGround; }
    }

    public PlanetPlatform OnPlatform
    {
        get { return planetPlatform; }
    }

    void Update()
    {  
        List<Collider2D> results = new List<Collider2D>();

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(groundLayer);
        float angle = Vector2.SignedAngle(Vector2.up, transform.up);
        if (Physics2D.OverlapBox(transform.position + (transform.up * offset.y), size, angle, contactFilter2D, results) > 0)
        {
            onGround = true;
            SetPlatform( results );
        }
        else
        {
            onGround = false;
            planetPlatform = null;
        }
    }

    private void SetPlatform (List<Collider2D> results)
    {
        foreach (Collider2D coll in results)
        {
            PlanetPlatform pp = coll.GetComponent<PlanetPlatform>();
            if (pp) 
            {
                planetPlatform = pp;
                return;
            }
        }
        planetPlatform = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;
        Gizmos.DrawWireCube(transform.position + (transform.up * offset.y), size);
    }
}
