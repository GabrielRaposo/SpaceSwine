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

    int worldId;
    int roundId;

    void Awake()
    {
        startingPosition = transform.position;

        moveToTarget = GetComponent<MoveToTarget>();
    }

    private void Start() 
    {
        worldId = 1;
        roundId = 0;

        Round round = GetComponentInParent<Round>();
        if (round)
            roundId = round.transform.GetSiblingIndex() + 1;

        //StartCoroutine( RaposUtil.Wait(30, DespawnLogic) );
        DespawnLogic();
    }

    private void DespawnLogic()
    {
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
        PlayerWallet.ChangeValue(value, worldId);
        CurrencyInstanceList.CountAsCollected( worldId, new Vector3(startingPosition.x, startingPosition.y, roundId) );

        gameObject.SetActive(false);
    }
}
