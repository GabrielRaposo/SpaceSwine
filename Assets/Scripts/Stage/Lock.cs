using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    //public bool isInternalLock;

    [SerializeField] private int startingHealth = 1;
    [SerializeField] private GameObject visualComponent;
    [SerializeField] private GameObject circleAura;
    [SerializeField] private GameObject idleParticles;
    [SerializeField] private ParticleSystem burstParticles;
    [SerializeField] private Collider2D col;
    [SerializeField] private LockGravityField lockGravityField;
    [SerializeField] private AK.Wwise.Event collectAKEvent;

    private int health;

    private Door _door;
    private Round _round;
    private Animator animator;

    public void Init (Door s, bool isInternal)
    {
        animator = GetComponent<Animator>();
        _door = s;

        health = startingHealth;
        col.enabled = !isInternal;
    }

    private void Update()
    {
        visualComponent.transform.rotation = Quaternion.identity;
    }

    protected virtual void Start()
    {
        _round = GetComponentInParent<Round>();
        _round.OnReset += OnReset;
    }

    public virtual bool Collect (Collectable collectable)
    {
        if (lockGravityField.CapturedCollectable != collectable && lockGravityField.CapturedCollectable != null)
            return false;

        PlaySound();
        collectable.gameObject.SetActive(false);
        
        health--;
        if (health < 1)
        {
            Unlock();

            if(_door)
                _door.TakeHealth();
        }

        return true;
    }
    
    public void Unlock()
    {
        if (animator)
            animator.SetTrigger("Unlock");
        
        if (idleParticles)
            idleParticles.SetActive(false);

        lockGravityField.gameObject.SetActive(false);
        col.enabled = false;
    }
    
    public void PlaySound()
    {
        collectAKEvent?.Post(gameObject);
    }

    public void CallParticleTrigger()
    {
        if (burstParticles)
            burstParticles.Play();

        if (circleAura)
            circleAura.SetActive(false);
    }

    public void AnimationEndTrigger()
    {
        visualComponent.SetActive(false);
    }

    public int GetHealth()
    {
        return health;
    }
    
    public void OnReset()
    {
        if (animator)
        {
            animator.SetBool("Unlock", false);
            animator.SetTrigger("Reset");
        }

        health = startingHealth;

        if (visualComponent)
            visualComponent.SetActive(true);
        
        if (circleAura)
            circleAura.SetActive(true);

        col.enabled = true;
        
        if (idleParticles)
            idleParticles.SetActive(true);

        if (burstParticles)
        {
            burstParticles.Stop();
            burstParticles.Clear();
        }
    
        lockGravityField.gameObject.SetActive(true);
    }

    private void OnDisable() 
    {
        if (burstParticles)
        {
            burstParticles.Stop();
            burstParticles.Clear();
        }
    }
}
