using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlatformerCharacter : SidewaysCharacter
{
    const float MAX_GRAVITY = 9.8f;

    [Header("Values")]
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [SerializeField] float jumpForce;

    [Header("References")]
    [SerializeField] Transform visualAnchor;
    [SerializeField] AK.Wwise.Event walkAKEvent;
    [SerializeField] AK.Wwise.Event shortHopAKEvent;
    [SerializeField] AK.Wwise.Event shortLandingAKEvent;

    [Header("Snap To Platform Values")]
    [SerializeField] Vector2 snapCheckPoint;
    [SerializeField] Vector2 snapCheckSize;
    [SerializeField] LayerMask groundLayer;

    [Header("Auto Climb Platform Values")]
    [SerializeField] Vector2 autoClimbCheckPoint;
    [SerializeField] Vector2 autoClimbCheckSize;

    bool onGround;

    float horizontalSpeed;
    float verticalSpeed;

    SpaceJumper spaceJumper;
    GravityInteraction gravityInteraction;
    CheckGround checkGround;
    CheckLedge checkLedge;
    CheckWall checkWall;
    PlayerAnimations playerAnimations;
    Rigidbody2D rb;

    void Awake()
    {
        spaceJumper = GetComponent<SpaceJumper>();
        gravityInteraction = GetComponent<GravityInteraction>();
        checkGround = GetComponent<CheckGround>();
        checkLedge = GetComponent<CheckLedge>();
        checkWall = GetComponent<CheckWall>();
        playerAnimations = GetComponent<PlayerAnimations>();

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() 
    {
        SetFacingRight(true);

        gravityInteraction.OnChangeGravityAnchor += (t) => 
        {   
            if (spaceJumper && spaceJumper.OnLaunch())
                return;

            if (t == null)
                return;

            verticalSpeed = horizontalSpeed = 0;
            transform.eulerAngles = Vector3.forward * gravityInteraction.AlignWithPlanet();
        };
    }

    protected override void SetFacingRight(bool value) 
    {
        base.SetFacingRight(value);
        visualAnchor.localEulerAngles = new Vector3 (visualAnchor.localEulerAngles.x, value ? 0 : 180, visualAnchor.localEulerAngles.z);
        //directionArrow.flipY = !value;
    }

    public void LookAtTarget(Transform target)
    {
        Vector3 anchoredPos = transform.position - target.position;
        Vector3 angledPos = RaposUtil.RotateVector(anchoredPos, - transform.eulerAngles.z);

        SetFacingRight(angledPos.x < 0);
    }

    public void HorizontalInput(float horizontalInput)
    {
        playerAnimations.horizontalInput = horizontalInput;

        if (horizontalInput != 0)
            SetFacingRight (horizontalInput > 0);

        if (gravityInteraction && !gravityInteraction.GetGravityArea().Item1)
            return;

        float targetSpeed = horizontalInput * speed;
        int direction = 0;
        if (horizontalSpeed != targetSpeed)
            direction = horizontalSpeed > targetSpeed ? -1 : 1;

        horizontalSpeed += acceleration * direction * Time.deltaTime;

        if (Mathf.Abs(horizontalSpeed - targetSpeed) < .2f)
            horizontalSpeed = targetSpeed;

        if (Mathf.Abs(horizontalSpeed) > speed)
            horizontalSpeed = direction * speed;

        if (checkWall && checkWall.MovingTowardsWall(horizontalSpeed))
        {
            horizontalSpeed = 0;
        }
    }

    public void JumpInput()
    {
        if (!checkGround.OnGround)
            return;

        verticalSpeed = jumpForce;
        shortHopAKEvent?.Post(gameObject);

        playerAnimations.SetJumpingState();
    }

    private void FixedUpdate() 
    {
        AutoClimbPlatform();
        SnapToPlatform();

        OnGroundLogic();
        LedgeLogic();
        UseCustomGravity();

        if (AllignFallDirectionWithGravity())
            return;

        // Default method;
        rb.velocity = RaposUtil.AlignWithTransform(transform, new Vector2 (horizontalSpeed, verticalSpeed));
    }

    private void OnGroundLogic()
    {
        bool previousState = onGround;
        onGround = checkGround.OnGround;
        
        if (onGround && verticalSpeed <= .1f)
        {
            if (!previousState)
                shortLandingAKEvent?.Post(gameObject);
            playerAnimations.SetLandedState();
        }

        playerAnimations.landedOnGround = (onGround && verticalSpeed <= .1f);

        if (onGround && verticalSpeed < 0)
        {
            Vector3 projection = Vector3.Project(rb.velocity, transform.up);
            if (Mathf.Abs(projection.y) <= .001f)
                verticalSpeed = 0;
        }

        gravityInteraction.SetLockIntoAngle(onGround); //Alterar se o jogo começar a usar fall throughs
    }

    private void LedgeLogic()
    {
        if (onGround)
            return;
            
        (bool isValid, bool left, bool right) ledge = checkLedge.OnLedge;
        if (!ledge.isValid)
            return;

        float pushForce = 2f;
        if (horizontalSpeed <= 0 && ledge.left)
            horizontalSpeed = -pushForce;

        if (horizontalSpeed >= 0 && ledge.right)
            horizontalSpeed = pushForce;
    }

    private void UseCustomGravity()
    {
        (bool valid, GravityArea area, float multiplier, bool onPlatform) gravity = gravityInteraction.GetGravityArea();
        if (!gravity.valid)
            return;

        verticalSpeed += Physics2D.gravity.y * gravity.area.intensity * gravity.multiplier * Time.fixedDeltaTime;
        verticalSpeed = Mathf.Clamp( verticalSpeed, - MAX_GRAVITY, MAX_GRAVITY );

        playerAnimations.verticalSpeed = verticalSpeed;
    }

    private bool AllignFallDirectionWithGravity()
    {
        (bool valid, GravityArea area, float multiplier, bool onPlatform) gravity = gravityInteraction.GetGravityArea();
        if (!gravity.valid || gravity.onPlatform)
            return false;

        Vector2 direction = (gravity.area.Center - transform.position).normalized;
        if (gravity.area.linear)
            direction = - gravity.area.transform.up;

        float angle = Vector2.SignedAngle(Vector2.down, direction);

        rb.velocity = RaposUtil.RotateVector(new Vector2 (horizontalSpeed, verticalSpeed), angle);

        return true;
    }

    public void KillInputs()
    {
        rb.velocity = Vector2.zero;

        horizontalSpeed = verticalSpeed = 0;
        playerAnimations.horizontalInput = 0;
    }

    public Vector2 LocalSpeed()
    {
        return new Vector2(horizontalSpeed, verticalSpeed);
    }

    public void PlayStepSound()
    {
        walkAKEvent?.Post(gameObject);
    }

    private void SnapToPlatform() 
    {
        if (checkGround.OnGround || checkGround.OnPlatform)
            return;

        if (horizontalSpeed == 0 || verticalSpeed > 0)
            return;

        // Gera raycasts que vão testar a plataformas próximas
        Vector2 testPoint = new Vector2(snapCheckPoint.x * (horizontalSpeed > 0 ? 1 : -1), snapCheckPoint.y);
        Vector2 anchoredPos = (Vector2) transform.position + RaposUtil.AlignWithTransform(transform, testPoint);
        Vector2 anchoredDirection = RaposUtil.AlignWithTransform
            (transform, new Vector2(snapCheckSize.x * (horizontalSpeed > 0 ? 1 : -1), 0));

        ExtDebug.DrawBox
        (   
            origin: anchoredPos + anchoredDirection, 
            halfExtents: snapCheckSize, 
            orientation: Quaternion.Euler(Vector3.forward * transform.eulerAngles.z),
            Color.green
        );

        List <Collider2D> results = new List<Collider2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(groundLayer);

        // Encontra componente de PlanetPlatform em colliders atingidos pelo raycast
        if (Physics2D.OverlapBox 
        (
            point: anchoredPos + anchoredDirection, 
            size: new Vector2 (snapCheckSize.x, .1f), 
            angle: transform.eulerAngles.z, 
            contactFilter2D, 
            results) 
                > 0
        )
        {
            PlanetPlatform planetPlatform = null;

            foreach (Collider2D coll2D in results) 
            {
                PlanetPlatform pp = coll2D.GetComponent<PlanetPlatform>();
                if (pp)
                {
                    planetPlatform = pp;
                    break;
                }
            }

        if (!planetPlatform)
            return;
            
        // Testa a posição do player na tela pra ver se ele ñão está logo acima da plataforma
        Vector2 diffToPlatform = (transform.position - planetPlatform.transform.position);
        float diffAngle = Vector2.SignedAngle(Vector2.up, planetPlatform.transform.up);
        diffToPlatform = RaposUtil.RotateVector(diffToPlatform, - diffAngle);

        float planetBorderX = planetPlatform.GetColliderSize().x - .2f;
        if (Mathf.Abs(diffToPlatform.x) < planetBorderX)
            return;

        // Faz o snap to platform
        float targetX = planetBorderX * (horizontalSpeed > 0 ? -1 : 1);

        transform.eulerAngles = planetPlatform.transform.eulerAngles;

        verticalSpeed = horizontalSpeed = 0;
        rb.velocity = Vector2.zero;

        Vector2 positionOffset = new Vector2(targetX, .5f);
        transform.position = 
            planetPlatform.transform.position + 
            (Vector3) RaposUtil.AlignWithTransform(planetPlatform.transform, positionOffset);
        }
    }

    private void AutoClimbPlatform() 
    {
        if (!checkGround.OnGround)
            return;

        if (horizontalSpeed == 0)
            return;

        Vector2 testPoint = new Vector2(autoClimbCheckPoint.x * (horizontalSpeed > 0 ? 1 : -1), autoClimbCheckPoint.y);
        Vector2 anchoredPos = (Vector2) transform.position + RaposUtil.AlignWithTransform(transform, testPoint);
        Vector2 anchoredDirection = RaposUtil.AlignWithTransform
            (transform, new Vector2(autoClimbCheckSize.x * (horizontalSpeed > 0 ? 1 : -1), 0));

        ExtDebug.DrawBox
        (   
            origin: anchoredPos + anchoredDirection, 
            halfExtents: autoClimbCheckSize, 
            orientation: Quaternion.Euler(Vector3.forward * transform.eulerAngles.z),
            Color.green
        );

        List <Collider2D> results = new List<Collider2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(groundLayer);

        if (Physics2D.OverlapBox 
        (
            point: anchoredPos + anchoredDirection, 
            size: new Vector2 (autoClimbCheckSize.x, .1f), 
            angle: transform.eulerAngles.z, 
            contactFilter2D, 
            results) 
                > 0
        )
        {
            PlanetPlatform planetPlatform = null;

            foreach (Collider2D coll2D in results) 
            {
                PlanetPlatform pp = coll2D.GetComponent<PlanetPlatform>();
                if (pp) 
                {
                    planetPlatform = pp;
                    break;
                }
            }

            if (!planetPlatform)
                return;

            Debug.Log("planetPlatform: " + planetPlatform);
            float planetBorderX = planetPlatform.GetColliderSize().x - .3f;

            // Faz o snap to platform
            float targetX = planetBorderX * (horizontalSpeed > 0 ? -1 : 1);

            transform.eulerAngles = planetPlatform.transform.eulerAngles;

            verticalSpeed = horizontalSpeed = 0;
            rb.velocity = Vector2.zero;

            Vector2 positionOffset = new Vector2(targetX, .5f);
            transform.position =
                planetPlatform.transform.position +
                (Vector3) RaposUtil.AlignWithTransform (planetPlatform.transform, positionOffset);
        }
    }

    private void OnDrawGizmosSelected() 
    {
        if (Application.isPlaying)
            return;

        //Vector2 testPoint = new Vector2(snapCheckPoint.x * (horizontalSpeed > 0 ? 1 : -1), snapCheckPoint.y);
        //Vector2 anchoredPos = (Vector2)transform.position + RaposUtil.AlignWithTransform(transform, testPoint);
        //Vector2 anchoredDirection = RaposUtil.AlignWithTransform
        //    (transform, new Vector2(snapCheckSize.x * (horizontalSpeed > 0 ? 1 : -1), 0));

        //ExtDebug.DrawBox
        //(
        //    origin: anchoredPos + anchoredDirection,
        //    halfExtents: snapCheckSize,
        //    orientation: Quaternion.Euler(Vector3.forward * transform.eulerAngles.z),
        //    Color.green
        //);

        Vector2 testPoint = new Vector2(autoClimbCheckPoint.x * (horizontalSpeed > 0 ? 1 : -1), autoClimbCheckPoint.y);
        Vector2 anchoredPos = (Vector2)transform.position + RaposUtil.AlignWithTransform(transform, testPoint);
        Vector2 anchoredDirection = RaposUtil.AlignWithTransform
            (transform, new Vector2(autoClimbCheckSize.x * (horizontalSpeed > 0 ? 1 : -1), 0));

        ExtDebug.DrawBox
        (
            origin: anchoredPos + anchoredDirection,
            halfExtents: autoClimbCheckSize,
            orientation: Quaternion.Euler(Vector3.forward * transform.eulerAngles.z),
            Color.yellow
        );
    }
}
