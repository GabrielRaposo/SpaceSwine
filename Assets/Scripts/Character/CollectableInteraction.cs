using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(SpaceJumper))]
[RequireComponent(typeof(PlayerAnimations))]
public class CollectableInteraction : MonoBehaviour
{
    [SerializeField] CollectablesQueue collectablesQueue;
    [SerializeField] float launchSpeed;
    [SerializeField] float cooldownDuration;
    [SerializeField] Transform holdAnchor;
    [SerializeField] PlayerDirectionDisplay directionDisplay; 
    [SerializeField] Transform arrowSprite;
    [SerializeField] ParticleSystem onCollectEffect;

    float cooldownCount;
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

    private void FixedUpdate() 
    {
        if (cooldownCount > 0)
        {
            cooldownCount -= Time.fixedDeltaTime;
            return;
        }
    
        if (current == null)
            GetFromQueue();
    }

    public void AxisInput (Vector2 axisInput)
    {
        //if (axisInput != Vector2.zero)
        this.axisInput = axisInput;

        if (directionDisplay)
            directionDisplay.UpdateDirection( !checkGround.OnGround, this.axisInput );
    }

    public void InteractInput()
    {
        if (!current)
            return;

        current.Interact(this);

        //if (current == null)
            //GetFromQueue();
        StartCooldownCount();
    }

    private void StartCooldownCount()
    {
        cooldownCount = cooldownDuration;
    }

    private void GetFromQueue()
    {
        current = collectablesQueue.GetFromQueue();
        if (current != null)
            SetCurrentCollectable(current);
    }

    public bool SetCurrentCollectable (Collectable collectable)
    {
        if (current && current != collectable)
            return AddToQueueInteraction(collectable);

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
        
        collectable.transform.localRotation = Quaternion.identity;

        onCollectEffect?.Play();
        current = collectable;

        return true;
    }

    public bool AddToQueueInteraction(Collectable collectable)
    {
        if (!collectablesQueue)
            return false;

        return collectablesQueue.AddToQueue(collectable);
    }

    public bool LaunchInput()
    {
        if (!current)
            return false;

        if (checkGround.OnGround)
        {
            Vector2 direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);
            LaunchCurrentIntoDirection(direction.normalized);
        }
        else
        {
            //Vector2 direction = axisInput;

            //Debug.Log("directionDisplay.transform.eulerAngles.z: " + directionDisplay.transform.eulerAngles.z);
            //Vector2 direction = RaposUtil.RotateVector(Vector2.up, directionDisplay.transform.eulerAngles.z);

            //if (axisInput == Vector2.zero)
            //    direction = RaposUtil.RotateVector(Vector2.up, directionDisplay.transform.eulerAngles.z);

            Vector2 direction = (arrowSprite.transform.position - transform.position).normalized;

            spaceJumper.LaunchIntoDirection(-direction.normalized);
            
            LaunchCurrentIntoDirection(direction.normalized);
        }

        return true;
    }

    public bool JetpackLaunch()
    {
        if (!current)
            return false;

        if (checkGround.OnGround)
        {
            Vector2 direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);
        }
        else
        {
            Vector2 direction = axisInput;
            if (axisInput == Vector2.zero)
                direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);

            spaceJumper.LaunchIntoDirection(-direction.normalized);
        }

        return true;
    }

    public Collectable UseCollectableWhileHolding()
    {
        if (current == null) return null;
        
        Collectable c = current;
        RemoveCollectable();

        //GetFromQueue();
        StartCooldownCount();

        return c;
    }

    public void RemoveCollectable()
    {
        current.transform.position = transform.position;
        playerAnimations.holding = false;
        current = null;
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
            hierarchyController.SetParentToRound();
            //hierarchyController.SetParent(null);

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
        
        collectablesQueue.ResetStates();

        playerAnimations.holding = false;
    }
}
