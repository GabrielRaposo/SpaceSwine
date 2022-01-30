using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyController : MonoBehaviour
{
    Vector3 originalPosition;
    Quaternion originalRotation;
    Transform originalParent;

    Transform roundTransform;
    
    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        Round round = GetComponentInParent<Round>();
        if (round)
        {
            round.OnReset += SetOriginalState;
            roundTransform = round.transform;
        }
    }

    public void SetParent(Transform t)
    {
        transform.parent = t;
    }

    public void SetParentToRound()
    {
        if (roundTransform)
            transform.parent = roundTransform;
        else
            transform.parent = null;
    }

    public void SetOriginalState()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.parent = originalParent;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
            rb.velocity = Vector2.zero;
    }
}
