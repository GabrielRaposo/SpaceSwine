using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(SpaceJumper))]
[RequireComponent(typeof(PlayerAnimations))]
[RequireComponent(typeof(PlatformerCharacter))]
public class CollectableInteraction : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float launchSpeed;
    [SerializeField] float cooldownDuration;
    [SerializeField] float airHoldDuration;
    
    [Header("Particles")]
    [SerializeField] ParticleSystem onCollectEffect;
    [SerializeField] ParticleSystem groundThrowEffect;
    [SerializeField] ParticleSystem airThrowEffect;
    
    [Header("References")]
    [SerializeField] CollectablesQueue collectablesQueue;
    [SerializeField] Transform holdAnchor;
    [SerializeField] PlayerDirectionDisplay directionDisplay; 
    //[SerializeField] Transform arrowSprite; // -- Era usado pra gerar o ângulo de arremesso

    [HideInInspector] public bool OnAirStall;

    float cooldownCount;
    Vector2 axisInput;
    Collectable current;

    Rigidbody2D rb;
    CheckGround checkGround;
    SpaceJumper spaceJumper;
    PlayerAnimations playerAnimations;
    PlatformerCharacter platformerCharacter;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        checkGround = GetComponent<CheckGround>();
        spaceJumper = GetComponent<SpaceJumper>();
        playerAnimations = GetComponent<PlayerAnimations>();
        platformerCharacter = GetComponent<PlatformerCharacter>();
    }

    private void Start() 
    {
        Health health = GetComponent<Health>();
        if (health)
        {
            health.OnDeathEvent += (f) => 
            { 
                StopAllCoroutines();
                directionDisplay.HideLine(blockfade: true);
            };
        }
    }

    private void FixedUpdate() 
    {
        QueueCooldown();

        if (OnAirStall)
            rb.velocity = Vector2.zero;
    }

    private void QueueCooldown()
    {
        if (cooldownCount > 0)
        {
            cooldownCount -= Time.fixedDeltaTime;
            return;
        }
    

        if (current == null)
        {
            GetFromQueue();
        }
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
 
        //GetFromQueue();
        StartCooldownCount();
    }

    private void StartCooldownCount()
    {
        //Debug.Log("StartCooldownCount");
        cooldownCount = cooldownDuration;
    }

    private void GetFromQueue()
    {
        //Debug.Log("Get from queue");
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

        FloatEffect floatEffect = collectable.GetComponentInChildren<FloatEffect>();
        if (floatEffect)
            floatEffect.enabled = false;

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

    public void LaunchInput()
    {
        if (!current)
            return;

        if (checkGround.OnGround || platformerCharacter.enabled) // -- TO-DO: trocar pelo estado do player
        {
            Vector2 direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);
            LaunchCurrentIntoDirection(direction.normalized);

            groundThrowEffect?.Play();

            playerAnimations.throwing = true;
        }
        else
            StartCoroutine(AirHoldSequence());
    }

    private IEnumerator AirHoldSequence()
    {
        OnAirStall = true;
        playerAnimations.airStall = true;

        //Vector2 direction = (arrowSprite.transform.position - transform.position).normalized;
        Vector2 direction = directionDisplay.GetDirection();
        spaceJumper.PointAndHoldIntoDirection(-direction.normalized);

        yield return new WaitForSeconds(airHoldDuration);
        
        // TO-DO: verificar se morreu

        airThrowEffect?.Play();

        spaceJumper.LaunchIntoDirection(-direction.normalized, multiplier: 1.0f, playLongJumpSound: false);
        LaunchCurrentIntoDirection(direction.normalized);

        OnAirStall = false;
        playerAnimations.airStall = false;

        yield return new WaitForEndOfFrame();
    }

    public bool JetpackLaunch()
    {
        if (!current)
            return false;

        if (checkGround.OnGround || platformerCharacter.enabled)
        {
            Vector2 direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);
        }
        else
        {
            Vector2 direction = axisInput;
            if (axisInput == Vector2.zero)
                direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);

            spaceJumper.LaunchIntoDirection(- direction.normalized);
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

        if (current == null)
            return;

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

        CollectableThrowable collectableThrowable = current.GetComponent<CollectableThrowable>();
        if (collectableThrowable)
            collectableThrowable.LaunchSetup();

        current = null;
    }

    public void ResetStates()
    {
        OnAirStall = false;

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
        playerAnimations.airStall = false;

        if (directionDisplay)
            directionDisplay.SetVisibility(true);

        enabled = true;
    }

    public void HideAimLine()
    {
        if (!directionDisplay)
            return;

        //Debug.Log("b");
        directionDisplay.HideLine(blockfade: true);
    }

    public void ToggleDirectionDisplay()
    {
        directionDisplay.gameObject.SetActive( !directionDisplay.gameObject.activeSelf );
    }
}
