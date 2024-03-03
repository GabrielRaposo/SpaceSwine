using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Bouncer : MonoBehaviour
{
    [SerializeField] float cooldown;
    [SerializeField] float width;

    [SerializeField] SpriteRenderer baseRenderer;
    [SerializeField] SpriteRenderer springRenderer;
    [SerializeField] BoxCollider2D boxCollider;

    float cooldownCount;
    Animator animator;

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        if (width <= 0)
            return;

        if (baseRenderer)
            baseRenderer.size = new Vector2 (width, baseRenderer.size.y);

        if (springRenderer)
            springRenderer.size = new Vector2 (width, springRenderer.size.y);

        if (boxCollider)
            boxCollider.size = new Vector2 (width, boxCollider.size.y);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public Vector2 ReflectVelocity (Vector2 velocity)
    {
        if (cooldownCount > 0)
            return velocity;

        cooldownCount = cooldown;

        animator.SetTrigger("PopBack");

        return Vector2.Reflect( velocity, RaposUtil.RotateVector(transform.up, 180) );
    }

    private void Update()
    {
        if (cooldownCount > 0)
            cooldownCount -= Time.deltaTime;
    }
}
