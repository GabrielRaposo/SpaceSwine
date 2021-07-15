using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncRadiusWithParent : MonoBehaviour
{
    [SerializeField] float difference;

    [SerializeField] CircleCollider2D baseCollider;
    [SerializeField] CircleCollider2D targetCollider;

    void Start()
    {
        SetColliders();
    }

    private void OnValidate() 
    {
        SetColliders();
    }

    private void SetColliders()
    {
        if (!baseCollider || !targetCollider)
            return;

        targetCollider.radius = baseCollider.radius + difference;
    }
}
