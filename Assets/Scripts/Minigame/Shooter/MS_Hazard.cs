using Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Hazard : MonoBehaviour
{
    const float LENGTH = 0.5f;

    [SerializeField] Vector2Int size;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D boxCollider;

    Animator animator;

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        spriteRenderer.size = (Vector2) size * LENGTH;
        boxCollider.size = (Vector2) size * LENGTH; 
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        MS_Session session = GetComponentInParent<MS_Session>();
        if (session)
        {
            session.OnVanish += Vanish;
            session.OnPreReset += Setup;
        }
    }

    private void Setup()
    {
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
    }

    public void Vanish()
    {
        animator.SetTrigger ("Vanish");
    }

    public void _AnimationCall()
    {
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
    }
}
