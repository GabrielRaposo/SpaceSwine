using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPlatform : MonoBehaviour
{
    Planet parentPlanet;    

    void Start()
    {
        parentPlanet = GetComponent<Planet>();
        if (parentPlanet)
            Debug.Log(parentPlanet.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
