using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuddingFlower : MonoBehaviour
{
    [SerializeField] Transform visualComponent;
    [SerializeField] ParticleSystem burstParticleSystem;

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
        if (visualComponent)
        {
            visualComponent.DOKill();
            visualComponent.localScale = Vector3.one;
        }

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

    public void PunchScale()
    {
        if (!visualComponent)
            return;

        visualComponent.DOKill();
        visualComponent.localScale = Vector3.one;

        visualComponent.DOPunchScale (Vector3.one * .3f, .15f);

        burstParticleSystem?.Play();
    }
}
