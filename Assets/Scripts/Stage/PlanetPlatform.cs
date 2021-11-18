using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPlatform : MonoBehaviour
{
    GravitationalPlanet parentPlanet;    

    void Start()
    {
        parentPlanet = GetComponent<GravitationalPlanet>();
        if (parentPlanet)
            Debug.Log(parentPlanet.name);
    }

}
