using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyController : MonoBehaviour
{
    Vector3 originalPosition;
    Quaternion originalRotation;
    Transform originalParent;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        Round round = GetComponentInParent<Round>();
        if (round)
            round.OnReset += SetOriginalState;
    }

    public void SetParent(Transform t)
    {
        transform.parent = t;
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
