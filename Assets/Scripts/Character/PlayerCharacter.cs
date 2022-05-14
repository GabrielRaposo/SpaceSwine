using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(CollectableInteraction))]
[RequireComponent(typeof(SpaceJumper))]
[RequireComponent(typeof(Health))]
// Classe responsável por conversar com interações externas com controladores e managers
public class PlayerCharacter : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D coll;
    SpriteRenderer spriteRenderer;

    LocalGameplayState gameplayState;
    PlatformerCharacter platformerCharacter;
    CollectableInteraction collectableInteraction;
    SpaceJumper spaceJumper;
    Health health;

    public static PlayerCharacter Instance;

    private void Awake() 
    {
        if (Instance) 
        {
            gameObject.SetActive(false);
            Debug.Log("Duplicate player instance found.");
            return;
        }

        Instance = this;
    }

    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>(); 
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        gameplayState = GetComponent<LocalGameplayState>();
        platformerCharacter = GetComponent<PlatformerCharacter>();
        collectableInteraction = GetComponent<CollectableInteraction>();
        spaceJumper = GetComponent<SpaceJumper>();
        health = GetComponent<Health>();
    }

    public void ResetStates()
    {
        platformerCharacter?.KillInputs();
        collectableInteraction?.ResetStates();
        spaceJumper?.ResetStates();
    }

    public void SpawnAt (Vector2 position, float rotation = 0)
    {
        ResetStates();

        transform.position = position;
        transform.eulerAngles = rotation * Vector3.forward;

        gameObject.SetActive(false);
        
        // Enter Spawn state

        gameObject.SetActive(true);
    }

    public void SetPhysicsBody (bool value)
    {
        Rigidbody2D rb2d = GetComponentInChildren<Rigidbody2D>();
        BoxCollider2D coll = GetComponentInChildren<BoxCollider2D>();

        coll.enabled = value;
        rb2d.bodyType = value ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
    }

    public void SetHiddenState (bool value) // -- É chamado por fora do código, pelo "ReactOnGGSEvent" no Player
    {
        if (rb)
            rb.bodyType = value ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;

        if (coll)
            coll.enabled = !value;
        
        if (spriteRenderer)
            spriteRenderer.enabled = !value;
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!gameplayState || gameplayState.state != GameplayState.Danger)
            return;

        if (!collision.CompareTag("GameplayArea"))
            return;

        health.Die();
    }
}
