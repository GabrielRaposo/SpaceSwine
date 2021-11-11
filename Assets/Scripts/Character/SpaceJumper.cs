using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(PlayerAnimations))]
public class SpaceJumper : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] AK.Wwise.Event longJumpAKEvent;
    [SerializeField] AK.Wwise.Event longLandAKEvent;

    bool onLaunch;

    PlatformerCharacter platformerCharacter;
    GravityInteraction gravityInteraction;
    CheckGround checkGround;
    PlayerAnimations playerAnimations;
    Rigidbody2D rb;

    void Start()
    {
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
        checkGround = GetComponent<CheckGround>();
        playerAnimations = GetComponent<PlayerAnimations>();

        rb = GetComponent<Rigidbody2D>();
    }

    public void JumpInput()
    {
        if (!checkGround.OnGround)
            return;

        SetLaunchState(true);

        Vector2 direction = RaposUtil.RotateVector(Vector2.up, transform.eulerAngles.z);
        rb.velocity = direction * speed;
    }

    private void SetLaunchState (bool value)
    {
        if (value) 
        {
            longJumpAKEvent?.Post(gameObject);
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

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (!onLaunch)
            return;

        if (collision.gameObject.layer != LayerMask.NameToLayer("Ground"))
            return;

        Vector3 topPosition = RaposUtil.RotateVector( Vector2.up * .25f, transform.eulerAngles.z );
        List <Collider2D> results = new List<Collider2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(groundLayer);
        if (Physics2D.OverlapBox(transform.position + topPosition, Vector2.one * .4f, transform.eulerAngles.z, contactFilter2D, results) < 1)
        {
            return;
        }

        Planet planet = collision.gameObject.GetComponent<Planet>();
        if (!planet)
        {
            PlanetBlock planetBlock = collision.gameObject.GetComponent<PlanetBlock>();
            if (planetBlock)
            {
                planet = collision.gameObject.GetComponentInParent<Planet>();
                if (!planet)
                    return;
            }
            else return;
        }

        Vector2 direction = (transform.position - planet.transform.position).normalized;
        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);

        longLandAKEvent?.Post(gameObject);

        SetLaunchState(false);
    }

    public void LaunchIntoDirection (Vector2 direction, float multiplier = 1.0f)
    {
        SetLaunchState(true);

        transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
        rb.velocity = direction * speed * multiplier;
    } 

    public bool OnLaunch()
    {
        return onLaunch;
    }

    public void ResetStates()
    {
        onLaunch = false;
        rb.velocity = Vector2.zero;
        SetLaunchState(false);
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
