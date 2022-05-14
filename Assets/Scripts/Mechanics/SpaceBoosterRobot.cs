using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBoosterRobot : MonoBehaviour
{
    [SerializeField] float delayMin;
    [SerializeField] float delayMax;

    bool animationPlaying;
    Animator animator;    
    SpriteRenderer spriteRenderer;

    private void Awake() 
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();    
    }

    void Start()
    {
        StartCoroutine (IdleAnimationLoop());

        Round round = GetComponentInParent<Round>();
        if (round)
            round.OnReset += ResetComponents;
    }

    private void ResetComponents()
    {
        StopAllCoroutines();


        StartCoroutine (IdleAnimationLoop());
    }

    private IEnumerator IdleAnimationLoop()
    {
        int random = Random.Range(0, 2);
        if (random == 0)
            spriteRenderer.flipX = !spriteRenderer.flipX;

        if (!animator || !spriteRenderer)
            yield break;

        while (true)
        {
            animator.SetTrigger("Reset");

            float delay = Random.Range(delayMin, delayMax);
            yield return new WaitForSeconds(delay);

            spriteRenderer.flipX = !spriteRenderer.flipX;

            animator.SetTrigger("Blink");
            animationPlaying = true;

            yield return new WaitWhile( () => animationPlaying );
        }
    }

    public void OnAnimationEnd()
    {
        animationPlaying = false;
    }
}
