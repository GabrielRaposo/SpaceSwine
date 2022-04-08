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

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!gameplayState || gameplayState.state != GameplayState.Danger)
            return;

        if (!collision.CompareTag("GameplayArea"))
            return;

        health.Die();
    }
}
