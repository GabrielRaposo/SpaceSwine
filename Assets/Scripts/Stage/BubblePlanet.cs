using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravitationalPlanet))]
public class BubblePlanet : MonoBehaviour, ConcealingBody
{
    [SerializeField] List<SpriteRenderer> renderers;
    [SerializeField] Collectable concealedCollectable;

    bool isActive;

    CircleCollider2D coll2D;
    GravitationalPlanet gravitationalPlanet;

    private void Awake() 
    {
        coll2D = GetComponent<CircleCollider2D>();
        gravitationalPlanet = GetComponent<GravitationalPlanet>();
    }

    private void Start() 
    {
        gravitationalPlanet.OnPlayerExitAction += Burst;

        Initiate();

        Round round = GetComponentInParent<Round>();
        if (!round)
            return;

        round.OnReset += Initiate;
    }

    private void Initiate()
    {
        if (concealedCollectable)
        {
            concealedCollectable.transform.position = transform.position;
            concealedCollectable.SetConcealingBody(this);
        }

        SetState(true);
    }

    public void Burst()
    {
        SetState(false);
    }

    private void SetState (bool value)
    {
        foreach (SpriteRenderer renderer in renderers)
            renderer.enabled = value;

        coll2D.enabled = value;

        isActive = value;
    }

    public bool IsActive() 
    {
        return isActive;
    }
}
