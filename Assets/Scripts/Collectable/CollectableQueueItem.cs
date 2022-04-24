using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableQueueItem : MonoBehaviour
{
    Collectable collectable;

    public void Initiate (Collectable collectable)
    {
        this.collectable = collectable;

        collectable.SetInteractable(false);
        collectable.transform.SetParent(transform);
        collectable.transform.position = transform.position;
        collectable.gameObject.SetActive(true);

        gameObject.SetActive(true);
    }

    public Collectable Use()
    {
        if (!collectable)
            return null;

        gameObject.SetActive(false);

        return collectable;
    } 
}
