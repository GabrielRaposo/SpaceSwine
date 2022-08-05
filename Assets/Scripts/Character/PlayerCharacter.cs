using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(PlayerAnimations))]
[RequireComponent(typeof(CollectableInteraction))]
[RequireComponent(typeof(SpaceJumper))]
[RequireComponent(typeof(Health))]

// Classe responsável por conversar com interações externas com controladores e managers
public class PlayerCharacter : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D coll;
    SpriteRenderer spriteRenderer;

    PlayerInput playerInput;
    LocalGameplayState gameplayState;
    PlatformerCharacter platformerCharacter;
    GravityInteraction gravityInteraction;
    PlayerAnimations playerAnimations;
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

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>(); 
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        playerInput = GetComponent<PlayerInput>();
        gameplayState = GetComponent<LocalGameplayState>();
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
        playerAnimations = GetComponent<PlayerAnimations>();
        collectableInteraction = GetComponent<CollectableInteraction>();
        spaceJumper = GetComponent<SpaceJumper>();
        health = GetComponent<Health>();
    }

    public void ResetStates()
    {
        if (coll) coll.enabled = true;
        if (playerInput) playerInput.enabled = true;

        platformerCharacter?.KillInputs();
        collectableInteraction?.ResetStates();
        spaceJumper?.ResetStates();
        //gravityInteraction.enabled = true;
    }

    public void DisableAllInteractions()
    { 
        if (playerInput) playerInput.enabled = false;

        if (platformerCharacter)
        {
            platformerCharacter.KillInputs();
            platformerCharacter.enabled = false;
        }

        if (collectableInteraction)
        {
            collectableInteraction.ResetStates();
            collectableInteraction.enabled = false;
        }

        if (spaceJumper)
        {
            spaceJumper.ResetStates();
            spaceJumper.enabled = false;
        }

        if (gravityInteraction)
        {
            gravityInteraction.enabled = false;
        }

        SetPhysicsBody(false);
        Rigidbody2D rb2d = GetComponentInChildren<Rigidbody2D>();
        rb2d.velocity = Vector2.zero;
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
        Debug.Log("SetHiddenState: " + value);

        if (rb)
            rb.bodyType = value ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;

        if (coll)
            coll.enabled = !value;
        
        if (spriteRenderer)
        {
            Debug.Log("spriteRenderer.name: " + spriteRenderer.name);
            spriteRenderer.enabled = !value;
        }
    }

    public void SetDeathEvent (UnityAction roundAction)
    {
        Health health = GetComponentInChildren<Health>();
        if (!health)
            return;

        health.OnDeathEvent = (deathFromDamage) => DeathSequence(deathFromDamage, roundAction);
    }

    private void DeathSequence (bool deathFromDamage, UnityAction roundAction)
    {
        if (deathFromDamage) // -- Morte tocando em spike
        {
            // TO-DO: Lockar inputs de menu

            Vector2 previousVelocity = rb.velocity.normalized;
            rb.velocity = previousVelocity * -.5f;
            
            if (coll) coll.enabled = false;
            if (playerInput) playerInput.enabled = false;

            playerAnimations.SetDeathState();

            RaposUtil.WaitSeconds(this, duration: 1.0f, () =>
            {
                roundAction.Invoke();
                // TO-DO: Liberar inputs de menu
            });
        }
        else // -- Morte saindo da tela
        {
            roundAction?.Invoke();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!gameplayState || gameplayState.state != GameplayState.Danger)
            return;

        if (!collision.CompareTag("GameplayArea"))
            return;

        health.Die(deathFromDamage: false);
    }
}
