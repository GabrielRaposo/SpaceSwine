using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableQueueItem : MonoBehaviour
{
    Collectable collectable;
    TransformTracker tracker;
    float trackPercent;

    Vector2 transitionStart;
    float transitionDuration = .2f;
    float t;
    bool onTransition;

    Vector2 previousPos = Vector2.zero;

    public void Initiate (Collectable collectable)
    {
        this.collectable = collectable;

        collectable.SetInteractable(false);
        collectable.UpdateSortingLayer(false);

        transform.position = collectable.transform.position;
        collectable.transform.SetParent(transform);
        collectable.transform.localEulerAngles = Vector3.zero;
        
        collectable.gameObject.SetActive(true);

        gameObject.SetActive(true);
    }

    public void SetTracker(TransformTracker tracker, float trackPercent)
    {
        if (tracker == this.tracker && trackPercent == this.trackPercent)
            return;

        this.tracker = tracker;
        this.trackPercent = trackPercent;

        if (Vector3.Distance (transform.position, tracker.GetPositionAtPercent(trackPercent)) > .1f)
        {
            t = 0;
            transitionStart = transform.position;
            onTransition = true;
        }
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

        if (onTransition)
        {
            transform.position = Vector3.Lerp(transitionStart, tracker.GetPositionAtPercent(trackPercent), t / transitionDuration);
            
            if (t >= transitionDuration)
                onTransition = false;

            t += Time.fixedDeltaTime;
            return;
        }

        if ( tracker.transform.position == (Vector3) previousPos )
            return;
        previousPos = tracker.transform.position;

        transform.position = tracker.GetPositionAtPercent(trackPercent);
        transform.eulerAngles = Vector2.zero;
    }

    private void OnDisable() 
    {
        tracker = null;
        trackPercent = 0;
    }
}
