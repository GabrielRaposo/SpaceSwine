using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanetBlock : MonoBehaviour
{
    [SerializeField] Vector2 size;

    [Header("Wall")]
    [SerializeField] BoxCollider2D wallCollider;
    [SerializeField] Transform wallVisual;

    [Header("Ground")]
    [SerializeField] BoxCollider2D groundCollider;
    [SerializeField] Transform groundVisual;

    Planet planet;

    void Start()
    {
        Setup();
    }

    #if UNITY_EDITOR
    private void Update() 
    {
        if (Application.isPlaying)
            return;

        Setup();
    }
    #endif

    private void Setup()
    {
        planet = GetComponentInParent<Planet>();
        if (!planet) 
            return;

        (float planet, float gravity) radius = planet.GetAttributes();
        Vector2 positionOffset = Vector2.up * ((size.y / 2) - .5f);

        transform.localPosition = Vector2.up * radius.planet;

        wallCollider.transform.localPosition = positionOffset;
        wallCollider.size = size;
        wallVisual.localScale = size;

        groundCollider.transform.localPosition = Vector2.up * ((size.y / 2) - .01f);
        groundCollider.transform.localPosition += (Vector3) positionOffset;
        groundCollider.size = new Vector2(size.x, .02f);
        groundVisual.localScale = groundCollider.size;
    }
}
