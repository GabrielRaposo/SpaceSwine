using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPlatform : MonoBehaviour
{
    BoxCollider2D boxCollider;
    GravitationalPlanet parentPlanet;    

    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        parentPlanet = GetComponent<GravitationalPlanet>();
        if (parentPlanet)
            Debug.Log(parentPlanet.name);
    }

    public Vector2 GetColliderSize()
    {
        if (!boxCollider)
            return Vector2.zero;

        return boxCollider.size;
    }
}
