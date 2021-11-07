using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public bool isInternalLock;
    private Star star;
    [SerializeField] private GameObject visualComponent;
    [SerializeField] private Collider2D col;

    public void Init(Star s, bool isInternal)
    {
        star = s;

        isInternalLock = isInternal;

        col.enabled = !isInternal;
    }

    public void Collect(Collectable collectable)
    {
        Unlock();
        
        if(star)
            star.TakeHealth();
        
        collectable.gameObject.SetActive(false);
        col.enabled = false;
        //collectAKEvent?.Post(gameObject);
    }
    
    public void Reset()
    {
        visualComponent.SetActive(true);
    }
    
    public void Unlock()
    {
        visualComponent.SetActive(false);
    }
    
}
