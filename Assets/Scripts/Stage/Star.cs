using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Star : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] int health;
    [SerializeField] float starRadius = 1f;

    [Header("References")]
    [SerializeField] RoundPortal portal;
    [SerializeField] TextMeshPro healthPreview;
    [SerializeField] CircleCollider2D mainCollider;
    [SerializeField] SpriteRenderer outlineVisualComponent;
    [SerializeField] SpriteRenderer insideVisualComponent;

    private int Health 
    {
        get
        {
            return health;
        }

        set 
        {
            health = value;
            if (healthPreview)
                healthPreview.text = health.ToString();
        }
    }
    Round round;
    int originalHealth;

    private void OnValidate() 
    {
        UpdateAttributes();
    }

    private void Start() 
    {
        originalHealth = Health;
        ResetStates();

        round = GetComponentInParent<Round>();    
        if (round)
            round.OnReset += ResetStates;
    }

    private void ResetStates()
    {
        gameObject.SetActive(true);
        Health = originalHealth;

        if (portal) 
        {
            portal.transform.SetParent(transform);
            portal.transform.position = transform.position;
            portal.gameObject.SetActive(false);
        }
    }

    private void UpdateAttributes()
    {
        if (mainCollider)
            mainCollider.radius = starRadius;

        if (outlineVisualComponent)
            outlineVisualComponent.transform.localScale = Vector3.one * starRadius * 2f;

        if (insideVisualComponent)
            insideVisualComponent.transform.localScale = outlineVisualComponent.transform.localScale - (Vector3.one * .04f);
    }

    public void Collect (Collectable collectable)
    {
        TakeHealth();

        collectable.gameObject.SetActive(false);
    }

    public void TakeHealth()
    {
        Health--;

        if (Health < 1)
            SpawnPortal();
    }

    public void SpawnPortal()
    {
        if (!portal || !round)
            return;

        portal.transform.SetParent(null);
        portal.transform.position = transform.position;
        portal.Setup
        (
            () => 
            {
                round.RoundCleared();
                portal.transform.SetParent(transform);
            }
        );

        gameObject.SetActive(false);
    }
}
