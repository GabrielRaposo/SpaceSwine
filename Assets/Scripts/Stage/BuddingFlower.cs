using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddingFlower : MonoBehaviour
{
    bool isOpen;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        Round round = GetComponentInParent<Round>();
        if (round)
        {
            round.OnReset += ResetComponents;
        }
    }

    private void ResetComponents()
    {
        SetState(false);
    }
    private void SetState(bool value)
    {
        isOpen = value;
        animator.SetBool("Open", value);
    }

    private void OnTriggerEnter2D (Collider2D collision) 
    {
        if (isOpen)
            return;

        if (!collision.CompareTag("Player"))
            return;

        SetState(true);
    }
}
