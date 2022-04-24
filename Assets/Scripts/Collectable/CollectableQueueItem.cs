using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableQueueItem : MonoBehaviour
{
    Collectable collectable;
    TransformTracker tracker;
    float trackPercent;

    Vector2 previousPos = Vector2.zero;

    public void Initiate (Collectable collectable)
    {
        this.collectable = collectable;

        collectable.SetInteractable(false);
        collectable.UpdateSortingLayer(false);
        collectable.transform.SetParent(transform);
        collectable.transform.localEulerAngles = Vector3.zero;
        collectable.transform.position = transform.position;
        collectable.gameObject.SetActive(true);

        gameObject.SetActive(true);
    }

    public void SetTracker(TransformTracker tracker, float trackPercent)
    {
        this.tracker = tracker;
        this.trackPercent = trackPercent;
    }

    public Collectable Use()
    {
        tracker = null;

        if (!collectable)
            return null;

        collectable.UpdateSortingLayer(true);
        gameObject.SetActive(false);

        return collectable;
    }

    private void FixedUpdate() 
    {
        if (!tracker)
            return;

        if ( tracker.transform.position == (Vector3) previousPos )
            return;
        previousPos = tracker.transform.position;

        transform.position = tracker.GetPositionAtPercent(trackPercent);
    }
}
