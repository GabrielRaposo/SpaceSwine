﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(SpaceJumper))]
public class CollectableInteraction : MonoBehaviour
{
    [SerializeField] float launchSpeed;
    [SerializeField] Transform holdAnchor;

    Vector2 axisInput;
    Collectable current;

    CheckGround checkGround;
    SpaceJumper spaceJumper;

    private void Start() 
    {
        checkGround = GetComponent<CheckGround>();
        spaceJumper = GetComponent<SpaceJumper>();
    }

    public void AxisInput(Vector2 axisInput)
    {
        this.axisInput = axisInput;
    }

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

    public void LaunchInput()
    {
        if (!current)
            return;

        if (checkGround.OnGround)
        {
            Vector2 direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);
            LaunchCurrentIntoDirection(direction.normalized);
        }
        else
        {
            Vector2 direction = axisInput;
            if (axisInput == Vector2.zero)
                direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);

            LaunchCurrentIntoDirection(direction.normalized);
            spaceJumper.LaunchIntoDirection(-direction.normalized);
        }
    }

    private void LaunchCurrentIntoDirection(Vector2 direction)
    {
        current.transform.position = transform.position;

        Rigidbody2D rb = current.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = direction * launchSpeed;
        }

        Collider2D coll = current.GetComponent<Collider2D>();
        if (coll)
        {
            coll.enabled = true;
        }

        current.transform.SetParent(null);
        current = null;
    }
}
