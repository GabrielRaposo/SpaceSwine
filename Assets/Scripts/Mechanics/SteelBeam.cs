using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelBeam : MonoBehaviour
{
    [SerializeField] Transform colliders;
    [SerializeField] List<GravitationalPlatform> platforms;


    void Start()
    {
        if (colliders != null)
        {
            SpriteRenderer[] spriteRenderers = colliders.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in spriteRenderers)
                sr.enabled = false;
        }
    }

    void Update()
    {
        
    }
}
