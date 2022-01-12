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

    private void Start()
    {
        _round = GetComponentInParent<Round>();

        _round.OnReset += Reset;
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
    
    public void Reset()
    {
        Debug.Log("Reset Lock");
        visualComponent.SetActive(true);
        particles.SetActive(true);
        col.enabled = true;
    }
    
    public void Unlock()
    {
        visualComponent.SetActive(false);
        particles.SetActive(false);
    }
    
}
