using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPlayerTrigger : MonoBehaviour
{
    [SerializeField] private Lock _lock;
    [SerializeField] private LockGravityField gravityField;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (_lock.GetHealth() < 1) return;
        if (gravityField.CapturedCollectable != null) return;

        var collectableInteraction = other.gameObject.GetComponent<CollectableInteraction>();
        var collectable = collectableInteraction.UseCollectableWhileHolding();
        
        if (collectable == null) return;

        _lock.Collect(collectable);
    }
}
