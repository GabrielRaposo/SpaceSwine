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
    [SerializeField] float speed;
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

    void Awake()
    {
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
        collectableInteraction = GetComponent<CollectableInteraction>();
        checkGround = GetComponent<CheckGround>();
        playerAnimations = GetComponent<PlayerAnimations>();

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() 
    {
        if (!onLaunch || flightLoopAKEvent == null)
            return;
        
        if (Time.timeScale != 0 && !flightLoopAKEvent.IsPlaying(gameObject))
            flightLoopAKEvent.Post(gameObject);

        if (Time.timeScale == 0)
            flightLoopAKEvent.Stop(gameObject);
    }

    public void JumpInput()
    {
        if (!checkGround.OnGround)
            return;

        LaunchIntoDirection(direction: RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z));
    }

    private void SetLaunchState (bool value, bool playLongJumpSound = true)
    {
        if (flightLoopAKEvent != null)
            flightLoopAKEvent.Stop(gameObject);

        if (value) 
        {
            flightLoopAKEvent?.Post(gameObject);
            if (playLongJumpSound) longJumpAKEvent?.Post(gameObject);
            playerAnimations.throwing = false;
            playerAnimations.SetLaunchedState();
            gravityInteraction.DettachFromSurfaces();
        }
        else       
        {
            playerAnimations.SetLandedState();
        }

        platformerCharacter.enabled = !value;
        gravityInteraction.enabled = !value;
        onLaunch = value;
    }

    private void OnCollisionEnter2D (Collision2D collision) 
    {
        if (!onLaunch)
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

        Vector2 direction = (transform.position - planet.transform.position).normalized;
        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);

        longLandVFX?.Play();
        longLandAKEvent?.Post(gameObject);

        SetLaunchState(false);
        //Debug.Log("Long land VFX");
    }

    private IEnumerator DelayCollisionInteraction(Collision2D collision)
    {
        yield return new WaitWhile( () => collectableInteraction.OnAirStall );
        OnCollisionEnter2D(collision);
    }

    public void PointAndHoldIntoDirection (Vector2 direction)
    {
        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
        rb.velocity = Vector2.zero;
    } 

    public void LaunchIntoDirection (Vector2 direction, float multiplier = 1.0f, bool playLongJumpSound = true)
    {
        SetLaunchState(true, playLongJumpSound);

        //direction = new Vector2(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y) );

        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
        //Debug.Log("direction: " + direction);
        //Debug.Log("transform.eulerAngles: " + transform.eulerAngles);
        rb.velocity = direction * speed * multiplier;
    } 

    public void RedirectIntoDirection (Vector2 direction)
    {
        SetLaunchState(true, playLongJumpSound: false);

        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
        rb.velocity = direction * rb.velocity.magnitude;
    }

    public bool OnLaunch()
    {
        return onLaunch;
    }

    public void ResetStates()
    {
        if (flightLoopAKEvent != null)
            flightLoopAKEvent.Stop(gameObject);

        onLaunch = false;
        rb.velocity = Vector2.zero;
        SetLaunchState(false);
    }

    private void OnDisable() 
    {
        if (flightLoopAKEvent != null)
            flightLoopAKEvent.Stop(gameObject);
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
