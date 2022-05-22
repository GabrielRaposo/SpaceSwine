using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyController : MonoBehaviour
{
    [SerializeField] bool useLocalPosition;

    Vector3 originalPosition;
    Quaternion originalRotation;
    Transform originalParent;

    Transform roundTransform;
    
    void Awake()
    {
        originalParent = transform.parent;
        originalPosition = useLocalPosition ? transform.localPosition : transform.position;
        originalRotation = transform.rotation;

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
        transform.parent = originalParent;

        if (useLocalPosition)
            transform.localPosition = originalPosition;
        else
            transform.position = originalPosition;

        transform.rotation = originalRotation;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
            rb.velocity = Vector2.zero;
    }
}
