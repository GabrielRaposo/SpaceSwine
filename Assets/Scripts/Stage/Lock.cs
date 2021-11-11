using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    //public bool isInternalLock;
    private Door _door;
    [SerializeField] private GameObject visualComponent;
    [SerializeField] private Collider2D col;

    public void Init(Door s, bool isInternal)
    {
        _door = s;

        //isInternalLock = isInternal;

        col.enabled = !isInternal;
    }

    public void Collect(Collectable collectable)
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
        visualComponent.SetActive(true);
        col.enabled = true;
    }
    
    public void Unlock()
    {
        visualComponent.SetActive(false);
    }
    
}
