using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(CollectableInteraction))]
[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(PlayerAnimations))]
public class SpaceJumper : MonoBehaviour
{
    public bool fallIntoGravityIfNothingElse;
    [SerializeField] float speed;
    [SerializeField] float gravitationalPull;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] ParticleSystem longLandVFX;
    [SerializeField] AK.Wwise.Event longJumpAKEvent;
    [SerializeField] AK.Wwise.Event longLandAKEvent;
    [SerializeField] AK.Wwise.Event flightLoopAKEvent;

    bool onLaunch;

    PlatformerCharacter platformerCharacter;
    GravityInteraction gravityInteraction;
    CollectableInteraction collectableInteraction;
    CheckGround checkGround;
    PlayerAnimations playerAnimations;
    Rigidbody2D rb;

    bool blockCollision;

    public bool BlockCollision { set { blockCollision = value; } }

    void Awake()
    {
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
        collectableInteraction = GetComponent<CollectableInteraction>();
        checkGround = GetComponent<CheckGround>();
        playerAnimations = GetComponent<PlayerAnimations>();

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        enabled = false;
    }

    private void Update() 
    {
        if (!onLaunch || flightLoopAKEvent == null)
            return;
        
        //Debug.Log($"A: Time.timeScale {Time.timeScale}, flightLoopAKEvent.IsPlaying(gameObject): {flightLoopAKEvent.IsPlaying(gameObject)}");
        if (Time.timeScale != 0 && !flightLoopAKEvent.IsPlaying(gameObject))
        {
            //Debug.Log("Post");
            flightLoopAKEvent.Post(gameObject);
        }

        if (Time.timeScale == 0)
        {
            //Debug.Log("Stop");
            flightLoopAKEvent.Stop(gameObject);
        }
    }

    
    private void FixedUpdate()
    {
        if (!fallIntoGravityIfNothingElse)
            return;

        if (gravitationalPull <= 0 || !onLaunch)
            return;

        float groundCastDistance = 2f;
        var groundCast = Physics2D.Raycast(transform.position, transform.up, groundCastDistance, groundLayer);
        if (!groundCast)
        {
            var gravityData = gravityInteraction.GetGravityArea();
            if (gravityData.isValid)
            {
                Vector2 direction = (gravityData.Area.transform.position - transform.position).normalized;
                if (Vector2.Angle(direction, transform.up) > 67.5f)
                    return;

                //Debug.Log("Is on gravity area: " + gravityData.Area.transform.parent.name);

                float speed = rb.velocity.magnitude;
                direction = rb.velocity + (direction * gravitationalPull);
                rb.velocity = direction.normalized * speed;
            }
        }
    }

    public void JumpInput()
    {
        if (!checkGround.OnGround)
            return;

        LaunchIntoDirection(direction: RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z));
    }

    private void SetLaunchState (bool value, bool playLongJumpSound = true)
    {
        onLaunch = value;
        KillFlightSound();

        if (value) 
        {
            //Debug.Log("SetLaunchState: true");
            flightLoopAKEvent?.Post(gameObject);
            if (playLongJumpSound) longJumpAKEvent?.Post(gameObject);
            playerAnimations.throwing = false;
            playerAnimations.SetLaunchedState();
            
            if (gravityInteraction.GBody != null)
                gravityInteraction.GBody.OnPlayerExitAction?.Invoke();
            
            gravityInteraction.DettachFromSurfaces();
        }
        else       
        {
            playerAnimations.SetLandedState();
        }
        enabled = value;

        platformerCharacter.enabled = !value;
        gravityInteraction.enabled = !value;
    }

    private void OnCollisionEnter2D (Collision2D collision) 
    {
        if (!onLaunch || blockCollision)
            return;

        if (collision.gameObject.layer != LayerMask.NameToLayer("Ground"))
            return;

        Vector3 topPosition = RaposUtil.RotateVector( Vector2.up * .25f, transform.eulerAngles.z );
        List <Collider2D> results = new List<Collider2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(groundLayer);
        if (Physics2D.OverlapBox(transform.position + topPosition, Vector2.one * .8f, transform.eulerAngles.z, contactFilter2D, results) < 1)
        {
            return;
        }

        GravitationalBody planet = collision.gameObject.GetComponent<GravitationalBody>();
        if (!planet)
        {
            PlanetBlock planetBlock = collision.gameObject.GetComponent<PlanetBlock>();
            if (planetBlock)
            {
                planet = collision.gameObject.GetComponentInParent<GravitationalBody>();
                if (!planet)
                    return;
            }
            else return;
        }

        if (collectableInteraction.OnAirStall)
        {
            StartCoroutine( DelayCollisionInteraction(collision) );
            return;
        }

        GravitationalPlatform gravitationalPlatform = planet.GetComponent<GravitationalPlatform>();
        if (gravitationalPlatform)
        {
            SnapIntoPlatform(gravitationalPlatform);
            transform.eulerAngles = gravitationalPlatform.transform.eulerAngles;
        }
        else
        {
            Vector2 direction = (transform.position - planet.transform.position).normalized;
            transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
        }

        GravityArea gravityArea = planet.GetComponentInChildren<GravityArea>();
        if (gravityArea)
            gravityInteraction.HardSetGravityArea (gravityArea);

        CustomSurface customSurface = collision.gameObject.GetComponent<CustomSurface>();
        if (customSurface == null)
        {
            longLandVFX?.Play();
            longLandAKEvent?.Post(gameObject);
        }
        else
        {
            customSurface.PlayLandEffects();
        }

        SetLaunchState(false);
    }

    private IEnumerator DelayCollisionInteraction(Collision2D collision)
    {
        yield return new WaitWhile( () => collectableInteraction.OnAirStall );
        OnCollisionEnter2D(collision);
    }

    private void SnapIntoPlatform (GravitationalPlatform platform)
    {
        (bool left, Transform t) anchor = platform.ClosestAnchor(transform.position);
        if (anchor.t == null)
            return;

        Vector3 positionOffset = transform.position - anchor.t.position;
        positionOffset = RaposUtil.RotateVector(positionOffset, - platform.transform.eulerAngles.z);

        if ( (anchor.left && positionOffset.x > 0) || (!anchor.left && positionOffset.x < 0) )
            return;

        transform.position = anchor.t.position;
    }

    public void PointAndHoldIntoDirection (Vector2 direction)
    {
        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
        rb.velocity = Vector2.zero;
    } 

    public void TryLaunchIntoDirection(Vector2 direction)
    {
        if (onLaunch)
            return;

        LaunchIntoDirection(direction);
    }

    public void LaunchIntoDirection (Vector2 direction, float multiplier = 1.0f, bool playLongJumpSound = true) 
    {
        SetLaunchState(true, playLongJumpSound);

        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
        //Debug.Log("direction: " + direction);
        //Debug.Log("transform.eulerAngles: " + transform.eulerAngles);
        rb.velocity = direction * speed * multiplier;
    } 

    public void RedirectIntoDirection (Vector2 direction, float speed = 0)
    {
        SetLaunchState(true, playLongJumpSound: false);

        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
        rb.velocity = direction * ( speed == 0 ? rb.velocity.magnitude : speed );
    }

    public bool OnLaunch()
    {
        return onLaunch;
    }

    public void ResetStates()
    {
        KillFlightSound();

        onLaunch = false;
        rb.velocity = Vector2.zero;
        SetLaunchState(false);
    }

    public void CancelOnLaunch()
    {
        onLaunch = false;
        KillFlightSound();
    }

    private void KillFlightSound()
    {
        //Debug.Log("KillFlightSound");

        if (flightLoopAKEvent != null)
            flightLoopAKEvent.Stop(gameObject);

        if (longJumpAKEvent != null)
            longJumpAKEvent.Stop(gameObject);
    }

    private void OnDisable() 
    {
        onLaunch = false;
        KillFlightSound();
    }

    private void OnDestroy() 
    {
        if (!Application.isPlaying)
            return;

        onLaunch = false;
        KillFlightSound();
    }

    private void OnDrawGizmos() 
    {
        if (!onLaunch)
            return;

        Vector3 topPosition = RaposUtil.RotateVector(Vector2.up * .25f, transform.eulerAngles.z);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + topPosition, .4f);
    }
}
