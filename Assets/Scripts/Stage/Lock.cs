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

    private Round _round;

    public void Init(Door s, bool isInternal)
    {
        _door = s;

        //isInternalLock = isInternal;

        col.enabled = !isInternal;
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
        //collectAKEvent?.Post(gameObject);
    }
    
    public void OnReset()
    {
        Debug.Log("Reset Lock");
        
        if(visualComponent)
            visualComponent.SetActive(true);
        
        col.enabled = true;
        
        if(particles)
            particles.SetActive(true);
    }
    
    public void Unlock()
    {
        visualComponent.SetActive(false);
        
        if(particles)
            particles.SetActive(false);
    }
    
}
