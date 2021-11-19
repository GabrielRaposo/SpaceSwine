using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveToTarget))]
public class CurrencyItem : MonoBehaviour
{
    [SerializeField] float value;
    bool collected;

    MoveToTarget moveToTarget;
    Vector2 startingPosition;

    void Awake()
    {
        startingPosition = transform.position;

        moveToTarget = GetComponent<MoveToTarget>();
    }

    private void Start() 
    {
        int worldId = 1;
        int roundId = 0;
        if (CurrencyInstanceList.CheckCollection( worldId, new Vector3(startingPosition.x, startingPosition.y, roundId) ))
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D (Collider2D collision) 
    {
        if (collected)
            return;

        if (!collision.CompareTag("Player"))
            return;

        PlayerCharacter playerCharacter = collision.GetComponent<PlayerCharacter>();
        if (!playerCharacter)
            return;

        collected = true;
        moveToTarget.SetTarget(playerCharacter.transform, OnCollect);
    }

    private void OnCollect()
    {
        int worldId = 1;
        int roundId = 0;
        PlayerWallet.ChangeValue(value, worldId);
        CurrencyInstanceList.CountAsCollected( worldId, new Vector3(startingPosition.x, startingPosition.y, roundId) );

        gameObject.SetActive(false);
    }
}
