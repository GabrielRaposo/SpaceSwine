using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warper : MonoBehaviour
{
    [HideInInspector] public Transform current;
    
    Warper other;
    
    public void Setup (Warper other)
    {
        this.other = other;
    }

    private void OnTriggerEnter2D (Collider2D collision) 
    {
        if (!other)
            return;

        if (current == collision.transform)
            return;

        if (collision.CompareTag("Player")) 
        {
            Warp (collision.transform);
            
            current = null;
            return;
        }

        CollectableThrowable collectable = collision.GetComponent<CollectableThrowable>();
        if (collectable)
        {
            collectable.NullifyPreviousHolder();
            Warp (collision.transform);

            current = null;
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.transform == current)
            current = null;
    }

    private void Warp (Transform target)
    {
        other.current = target;
        target.position = other.transform.position;

        // Chama feedback visual nos dois
    }
}
