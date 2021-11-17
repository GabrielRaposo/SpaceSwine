using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveToTarget))]
public class CurrencyItem : MonoBehaviour
{
    [SerializeField] float value;
    bool collected;

    MoveToTarget moveToTarget;

    void Awake()
    {
        moveToTarget = GetComponent<MoveToTarget>();
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
        PlayerWallet.ChangeValue(value, worldId);

        gameObject.SetActive(false);
    }
}
