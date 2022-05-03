using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableQueueItem : MonoBehaviour
{
    Collectable collectable;
    TransformTracker tracker;
    float trackPercent;

    Vector2 transitionStart;
    float transitionDuration = .3f;
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
        
        FloatEffect floatEffect = collectable.GetComponentInChildren<FloatEffect>();
        if (floatEffect)
            floatEffect.LineUp();

        collectable.gameObject.SetActive(true);

        tracker = null;
        trackPercent = 0;

        gameObject.SetActive(true);
    }

    public void SetTracker(TransformTracker tracker, float trackPercent)
    {
        if (tracker == this.tracker && trackPercent == this.trackPercent)
            return;

        this.tracker = tracker;
        this.trackPercent = trackPercent;

        (Vector3 Position, float Angle) data = tracker.GetDataAtPercent(trackPercent);
        if (Vector3.Distance (transform.position, data.Position) > .1f)
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

        (Vector3 Position, float Angle) data = tracker.GetDataAtPercent(trackPercent);

        if (onTransition)
        {
            transform.position = Vector3.Lerp(transitionStart, data.Position, t / transitionDuration);
            
            if (t >= transitionDuration)
                onTransition = false;

            t += Time.fixedDeltaTime;
            return;
        }

        if ( tracker.transform.position == (Vector3) previousPos )
            return;
        previousPos = tracker.transform.position;

        transform.position = data.Position;
        //transform.eulerAngles = data.Angle * Vector3.forward;
    }
}
