using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(SpaceJumper))]
[RequireComponent(typeof(PlayerAnimations))]
public class CollectableInteraction : MonoBehaviour
{
    [SerializeField] float launchSpeed;
    [SerializeField] Transform holdAnchor;

    Vector2 axisInput;
    Collectable current;

    CheckGround checkGround;
    SpaceJumper spaceJumper;
    PlayerAnimations playerAnimations;

    private void Start() 
    {
        checkGround = GetComponent<CheckGround>();
        spaceJumper = GetComponent<SpaceJumper>();
        playerAnimations = GetComponent<PlayerAnimations>();
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

    public bool SetCurrentCollectable (Collectable collectable)
    {
        if (current)
            return false;

        playerAnimations.holding = true;

        Transform t = holdAnchor;
        if (!holdAnchor)
            t = transform;        

        Collider2D collider2D = collectable.GetComponent<Collider2D>();
        if (collider2D)
            collider2D.enabled = false;

        HierarchyController hierarchyController = collectable.GetComponent<HierarchyController>();
        if (hierarchyController) 
            hierarchyController.SetParent(t);
        
        collectable.transform.position = t.position;

        current = collectable;
        return true;
    }

    public bool LaunchInput(bool releaseInteractable = true)
    {
        if (!current)
            return false;

        if (checkGround.OnGround)
        {
            Vector2 direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);
            if(releaseInteractable)
                LaunchCurrentIntoDirection(direction.normalized);
        }
        else
        {
            Vector2 direction = axisInput;
            if (axisInput == Vector2.zero)
                direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);

            if(releaseInteractable)
                LaunchCurrentIntoDirection(direction.normalized);
            
            spaceJumper.LaunchIntoDirection(-direction.normalized);
        }

        return true;
    }

    private void LaunchCurrentIntoDirection(Vector2 direction)
    {
        playerAnimations.holding = false;

        current.transform.position = transform.position;

        Rigidbody2D rb = current.GetComponent<Rigidbody2D>();
        if (rb)
            rb.velocity = direction * launchSpeed;

        Collider2D coll = current.GetComponent<Collider2D>();
        if (coll)
            coll.enabled = true;

        HierarchyController hierarchyController = current.GetComponent<HierarchyController>();
        if (hierarchyController) 
            hierarchyController.SetParent(null);

        current = null;
    }

    public void ResetStates()
    {
        if (current != null)
        {
            Rigidbody2D rb = current.GetComponent<Rigidbody2D>();
            if (rb)
                rb.velocity = Vector2.zero;

            HierarchyController hierarchyController = current.GetComponent<HierarchyController>();
            if (hierarchyController) 
                hierarchyController.SetOriginalState();

            current = null;
        }

        // current = null;
        playerAnimations.holding = false;
    }
}
