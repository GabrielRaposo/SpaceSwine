﻿using System.Collections;
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
    [SerializeField] Vector2 snapCheckDirection;

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
        if (checkGround.OnGround)
            return;

        if (horizontalSpeed == 0 || verticalSpeed > 0)
            return;

        Vector2 anchoredPos = (Vector2) transform.position + RaposUtil.AlignWithTransform(transform, snapCheckPoint); 
        Vector2 anchoredDirection = RaposUtil.AlignWithTransform(transform, 
            new Vector2(snapCheckDirection.x * (horizontalSpeed > 0 ? 1 : -1), snapCheckDirection.y ));

        Debug.DrawLine(anchoredPos, anchoredPos + anchoredDirection, Color.green);

    }

    //private void OnDrawGizmosSelected() 
    //{
    //    if (horizontalSpeed == 0)
    //        return;

    //    Vector2 anchoredPos = (Vector2) transform.position + RaposUtil.AlignWithTransform(transform, snapCheckPoint); 
    //    Vector2 anchoredDirection = RaposUtil.AlignWithTransform(transform, 
    //        new Vector2(snapCheckDirection.x * (horizontalSpeed > 0 ? 1 : -1), snapCheckDirection.y ));

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(anchoredPos, anchoredPos + anchoredDirection);
    //}
}
