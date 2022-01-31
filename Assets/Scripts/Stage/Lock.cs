using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    //public bool isInternalLock;
    private Door _door;
    [SerializeField] private GameObject visualComponent;
    [SerializeField] private GameObject particles;
    [SerializeField] private Collider2D col;
    [SerializeField] private LockGravityField lockGravityField;
    [SerializeField] private AK.Wwise.Event collectAKEvent;

    private Round _round;

    public void Init(Door s, bool isInternal)
    {
        _door = s;

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

    public virtual void Collect(Collectable collectable)
    {
        Unlock();
        
        if(_door)
            _door.TakeHealth();
        
        collectable.gameObject.SetActive(false);
        col.enabled = false;
        lockGravityField.gameObject.SetActive(false);
        collectAKEvent?.Post(gameObject);
    }
    
    public void OnReset()
    {
        if(visualComponent)
            visualComponent.SetActive(true);
        
        col.enabled = true;
        
        if(particles)
            particles.SetActive(true);
        
        lockGravityField.gameObject.SetActive(true);
    }
    
    public void Unlock()
    {
        visualComponent.SetActive(false);
        
        if(particles)
            particles.SetActive(false);
    }
    
}
