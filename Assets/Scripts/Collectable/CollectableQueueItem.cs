using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableQueueItem : MonoBehaviour
{
    Collectable collectable;
    TransformTracker tracker;
    float trackPercent;

    public void Initiate (Collectable collectable, TransformTracker tracker, float trackPercent)
    {
        this.collectable = collectable;
        this.tracker = tracker;
        this.trackPercent = trackPercent;

        collectable.SetInteractable(false);
        collectable.transform.SetParent(transform);
        collectable.transform.position = transform.position;
        collectable.gameObject.SetActive(true);

        gameObject.SetActive(true);
    }

    public Collectable Use()
    {
        tracker = null;

        if (!collectable)
            return null;

        gameObject.SetActive(false);

        return collectable;
    }

    private void FixedUpdate() 
    {
        if (!tracker)
            return;

        transform.position = tracker.GetPositionAtPercent(trackPercent);
    }
}
