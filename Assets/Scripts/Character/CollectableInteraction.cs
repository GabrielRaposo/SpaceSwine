using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckGround))]
public class CollectableInteraction : MonoBehaviour
{
    [SerializeField] Transform holdAnchor;

    Collectable current;

    public void InteractInput()
    {
        if (!current)
            return;

        current.Interact(this);
    }

    public void SetCurrentCollectable (Collectable collectable)
    {
        Transform t = holdAnchor;
        if (!holdAnchor)
            t = transform;        

        Collider2D collider2D = collectable.GetComponent<Collider2D>();
        if (collider2D)
            collider2D.enabled = false;

        collectable.transform.position = t.position;
        collectable.transform.SetParent(t);

        current = collectable;
    }
}
