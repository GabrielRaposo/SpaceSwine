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
    GravitationalBody planet;

    Color debugCollisionColor = Color.blue;

    public bool OnGround
    {
        get { return onGround; }
    }

    public PlanetPlatform OnPlatform
    {
        get { return planetPlatform; }
    }

    public GravitationalBody OnPlanet
    {
        get { return planet; }
    }

    public (bool value, CustomSurface customSurface) OnCustomSurface
    {
        get 
        {
            if (!onGround)
                return (false, null);

            if (OnPlanet && OnPlanet.GetComponent<CustomSurface>() != null)
                return (true, OnPlanet.GetComponent<CustomSurface>());

            return (false, null);
        }
    }

    void Update()
    {  
        List<Collider2D> results = new List<Collider2D>();

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(groundLayer);
        float angle = Vector2.SignedAngle(Vector2.up, transform.up);
        if (Physics2D.OverlapBox(transform.position + (transform.up * offset.y), size, angle, contactFilter2D, results) > 0)
        {
            //Debug.Log("update, results: " + results.Count);
            onGround = true;
            SetPlatform( results );
            SetPlanet( results );
        }
        else
        {
            onGround = false;
            planetPlatform = null;
            planet = null;
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

    private void SetPlanet (List<Collider2D> results)
    {
        foreach (Collider2D coll in results)
        {
            GravitationalBody b = coll.GetComponent<GravitationalBody>();
            if (b) 
            {
                planet = b;
                return;
            }
        }
        planet = null;
    }

    private void OnDisable() 
    {
        planetPlatform = null;
        planet = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;
        Gizmos.DrawWireCube (transform.position + (transform.up * offset.y), size);
    }
}
