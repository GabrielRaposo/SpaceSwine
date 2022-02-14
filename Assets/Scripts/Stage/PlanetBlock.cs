using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanetBlock : MonoBehaviour
{
    [SerializeField] Vector2 size;
    [SerializeField] Vector2 colliderOffset;
    [SerializeField] float radiusOffset;
    [SerializeField] bool useSpriteSize;

    [Header("Wall")]
    [SerializeField] BoxCollider2D wallCollider;
    [SerializeField] SpriteRenderer wallVisual;

    [Header("Ground")]
    [SerializeField] BoxCollider2D groundCollider;
    [SerializeField] Transform groundVisual;

    GravitationalPlanet planet;

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
        planet = GetComponentInParent<GravitationalPlanet>();
        if (!planet) 
            return;

        (float planet, float gravity) radius = planet.GetAttributes();
        Vector2 positionOffset = Vector2.up * ((size.y / 2) - .5f);

        transform.localPosition = Vector2.up * (radius.planet + radiusOffset);

        wallCollider.enabled = size.y > .65f;


        if (!useSpriteSize) 
        { 
            wallCollider.transform.localPosition = positionOffset;
            wallCollider.size = size + colliderOffset;

            wallVisual.drawMode = SpriteDrawMode.Simple;
            wallVisual.transform.localScale = size;

            groundCollider.transform.localPosition = Vector2.up * (((size + colliderOffset).y / 2) - .01f);
            groundCollider.transform.localPosition += (Vector3) positionOffset;
            groundCollider.size = new Vector2(size.x + colliderOffset.x, .02f);
            groundVisual.localScale = groundCollider.size;
        }
        else
        {
            positionOffset += Vector2.up * (colliderOffset.y / 2);
            wallCollider.transform.localPosition = positionOffset;
            wallCollider.size = size + colliderOffset;

            wallVisual.drawMode = SpriteDrawMode.Tiled;
            wallVisual.transform.localPosition = - positionOffset;
            wallVisual.transform.localScale = Vector2.one;
            wallVisual.size = new Vector2(1.25f, size.y);

            groundCollider.transform.localPosition = Vector2.up * (((size + colliderOffset).y / 2) - .01f);
            groundCollider.transform.localPosition += (Vector3) positionOffset;
            groundCollider.size = new Vector2(size.x + colliderOffset.x, .02f);
            groundVisual.localScale = groundCollider.size;
        }
    }

    public float GetSize()
    {
        return size.y;
    }
}
