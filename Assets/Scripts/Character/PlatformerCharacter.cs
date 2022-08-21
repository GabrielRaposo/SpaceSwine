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

    [Header("Dynamic Input Values")]
    [SerializeField] bool dynamicControls;
    [SerializeField] [Range(0f, 1f)] float dynamicInputThreshold;
    [SerializeField] [Range(0, 30)] int holdAnchorDuration;

    [Header("References")]
    [SerializeField] Transform visualAnchor;
    [SerializeField] ParticleSystem walkVFX;
    [SerializeField] ParticleSystem shortLandingVFX;
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

    [HideInInspector] public bool BlockLandFeedback; 
    bool onGround;

    Vector2 heldInput;
    float moveInputRotationAnchor;
    float lastValidAnchor;
    int holdAnchorCount;

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

    private void OnEnable() 
    {
        heldInput = Vector2.zero;    
        moveInputRotationAnchor = lastValidAnchor = holdAnchorCount = 0;
        horizontalSpeed = 0;
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
        if (!enabled) return;
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

    public void HorizontalInput (Vector2 axisInput)
    {
        float horizontalInput = axisInput.x;

        if (dynamicControls) 
        {
            axisInput = ConvertAxisInput( axisInput );
            horizontalInput = axisInput.x;
        }

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

    private Vector2 ConvertAxisInput(Vector2 rawInput)
    {
        #if UNITY_EDITOR
            Vector2 basePos = transform.position;
            Debug.DrawLine(basePos, basePos + rawInput, Color.yellow);
        #endif

        float anchor = transform.eulerAngles.z;

        if (rawInput != Vector2.zero) 
        {
            // -- Se o input não é zero, ou é diferente do anterior, ou ele estava anteriormente nulo: troca a âncora
            /**
            if (  
                    ( heldInput == Vector2.zero        && holdAnchorCount < 1 ) || 
                    ( !IsTooClose(heldInput, rawInput) && holdAnchorCount < 1 ) ||
                    IsTooClose(heldInput, rawInput)
               )
            **/
            if ( (heldInput == Vector2.zero || !IsCloseEnough(heldInput, rawInput)) && holdAnchorCount < 1)
            {
                moveInputRotationAnchor = transform.eulerAngles.z;
            }
            if (heldInput != rawInput && IsCloseEnough(heldInput, rawInput))
            {
                moveInputRotationAnchor = transform.eulerAngles.z;
            }
            anchor = moveInputRotationAnchor;
            heldInput = rawInput;

            holdAnchorCount = holdAnchorDuration; 
        } 
        else
        {
            heldInput = Vector2.zero;
        }

        Vector2 output = FilterThroughAnchor(rawInput, anchor);

        if (output.x == 0) // -- Se o player está no threshold de "inválido" do X
        {
            // -- Se estiver no setor norte do planeta, usa 0 como lastValidAnchor
            float roundedAngle = transform.eulerAngles.z % 360f;
            float angleOffset = 95f;
            if ( roundedAngle <= angleOffset || roundedAngle >= 360 - angleOffset )
            {
                lastValidAnchor = 0;
            }
            
            output = FilterThroughAnchor(rawInput, lastValidAnchor); // -- Faz o movimento com o último válido

            if (output.x != 0) // -- Se não estiver mais no threshold inválido, guarda o novo anchor
            {
                moveInputRotationAnchor = lastValidAnchor;
            }
            else if (output != Vector2.zero)
            {
                output = FilterThroughAnchor(rawInput, anchor, customThrehold: 0);
            }
            /**
            //else if (output != Vector2.zero) // -- Se o input ainda está perpendicular demais, tenta se mover com os verticais
            //{
            //    float angleWithRaw = Vector2.SignedAngle(transform.up, output);
            //    Debug.Log("angleWithRaw: " + angleWithRaw );
            //    if (angleWithRaw < 0)
            //    {
            //        float a = 270 * (roundedAngle <= angleOffset || roundedAngle >= 360 - angleOffset ? 1 : -1);
            //        moveInputRotationAnchor = lastValidAnchor = a % 360;
            //    }
            //    else
            //    {
            //        float a = 90 * (roundedAngle <= angleOffset || roundedAngle >= 360 - angleOffset ? 1 : -1);
            //        moveInputRotationAnchor = lastValidAnchor = a % 360;
            //    }
            //    output = FilterThroughAnchor(rawInput, lastValidAnchor); // -- Faz o movimento com o último válido
            //}
            **/
        }
        else
            lastValidAnchor = moveInputRotationAnchor;

        return output;
    }

    private bool IsCloseEnough(Vector2 A, Vector2 B)
    {
        A = A.To8Directions();
        B = B.To8Directions();

        if (A.x == -B.x || A.y == -B.y)
            return false;

        //float angleA = Vector2.SignedAngle(Vector2.up, A);
        //if (angleA < 360) angleA += 360;
        //angleA %= 360;

        //float angleB = Vector2.SignedAngle(Vector2.up, B);
        //if (angleB < 360) angleB += 360;
        //angleB %= 360;

        //if (Mathf.Abs(angleA - angleB) > 90)
        //    return false;

        return true;
    }

    private Vector2 FilterThroughAnchor (Vector2 rawInput, float anchor)
    {
        Vector2 anchoredInput = RaposUtil.RotateVector(rawInput, -anchor);

        Vector2 output = Vector2.zero;
        if (Mathf.Abs (anchoredInput.x) > dynamicInputThreshold)
            output = Vector2.right * (anchoredInput.x > 0 ? 1 : -1);
        if (Mathf.Abs (anchoredInput.y) > dynamicInputThreshold)
            output = new Vector2(output.x, anchoredInput.y > 0 ? 1 : -1);  

        return output;
    }

    private Vector2 FilterThroughAnchor (Vector2 rawInput, float anchor, float customThrehold)
    {
        Vector2 anchoredInput = RaposUtil.RotateVector(rawInput, -anchor);

        Vector2 output = Vector2.zero;
        if (Mathf.Abs (anchoredInput.x) > customThrehold)
            output = Vector2.right * (anchoredInput.x > 0 ? 1 : -1);
        if (Mathf.Abs (anchoredInput.y) > customThrehold)
            output = new Vector2(output.x, anchoredInput.y > 0 ? 1 : -1);  

        return output;
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

        EffectsSpawn();

        if (AllignFallDirectionWithGravity())
            return;

        // Default method;
        if (rb.bodyType != RigidbodyType2D.Dynamic)
            return;
        
        rb.velocity = RaposUtil.AlignWithTransform(transform, new Vector2 (horizontalSpeed, verticalSpeed));
    }

    private void OnGroundLogic()
    {
        bool previousState = onGround;
        onGround = checkGround.OnGround;
        
        if (onGround && verticalSpeed <= .1f)
        {
            if (!previousState) 
            {
                if (!BlockLandFeedback)
                {
                    shortLandingAKEvent?.Post(gameObject);
                    shortLandingVFX?.Play();
                }
            }
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

        if (rb.bodyType != RigidbodyType2D.Static)
            rb.velocity = RaposUtil.RotateVector(new Vector2 (horizontalSpeed, verticalSpeed), angle);

        return true;
    }

    public void KillInputs()
    {
        rb.velocity = Vector2.zero;

        horizontalSpeed = verticalSpeed = 0;
        playerAnimations.horizontalInput = 0;

        heldInput = Vector2.zero;    
        moveInputRotationAnchor = lastValidAnchor = holdAnchorCount = 0;
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

            //Debug.Log("planetPlatform: " + planetPlatform);
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

    public void StandStillState()
    {
        playerAnimations.landedOnGround = true;
        enabled = false;
    }

    private void EffectsSpawn()
    {
        if (!walkVFX)
            return;

        if (playerAnimations.landedOnGround && horizontalSpeed != 0)
        {
            if (!walkVFX.isPlaying)
                walkVFX.Play();
        }
        else walkVFX.Stop();
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

    private void OnDisable() 
    {
        walkVFX?.Stop();    
    }
}
