using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    //public bool isInternalLock;

    [SerializeField] private int startingHealth = 1;
    [SerializeField] private GameObject visualComponent;
    [SerializeField] private GameObject particles;
    [SerializeField] private Collider2D col;
    [SerializeField] private LockGravityField lockGravityField;
    [SerializeField] private AK.Wwise.Event collectAKEvent;

    private int health;

    private Door _door;
    private Round _round;

    public void Init (Door s, bool isInternal)
    {
        _door = s;

        health = startingHealth;
        //isInternalLock = isInternal;

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

        collectAKEvent?.Post(gameObject);
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
        visualComponent.SetActive(false);
        
        if (particles)
            particles.SetActive(false);

        col.enabled = false;
        lockGravityField.gameObject.SetActive(false);
    }
    
    public int GetHealth()
    {
        return health;
    }
    
    public void OnReset()
    {
        health = startingHealth;

        if(visualComponent)
            visualComponent.SetActive(true);
        
        col.enabled = true;
        
        if(particles)
            particles.SetActive(true);
        
        lockGravityField.gameObject.SetActive(true);
    }
}
