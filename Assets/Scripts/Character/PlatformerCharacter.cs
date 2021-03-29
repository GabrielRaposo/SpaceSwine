using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(CheckLedge))]
[RequireComponent(typeof(CheckWall))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacter : SidewaysCharacter
{
    const float MAX_GRAVITY = 9.8f;

    [Header("Values")]
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [SerializeField] float jumpForce;

    [Header("References")]
    [SerializeField] SpriteRenderer directionArrow;

    bool onGround;

    float horizontalSpeed;
    float verticalSpeed;

    GravityInteraction gravityInteraction;
    CheckGround checkGround;
    CheckLedge checkLedge;
    CheckWall checkWall;
    Rigidbody2D rb;

    void Awake()
    {
        gravityInteraction = GetComponent<GravityInteraction>();
        checkGround = GetComponent<CheckGround>();
        checkLedge = GetComponent<CheckLedge>();
        checkWall = GetComponent<CheckWall>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() 
    {
        SetFacingRight(true);
    }

    protected override void SetFacingRight(bool value) 
    {
        base.SetFacingRight(value);
        directionArrow.flipY = !value;
    }

    public void HorizontalInput(float horizontalInput)
    {
        if (horizontalInput != 0)
            SetFacingRight (horizontalInput > 0);

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
    }

    private void FixedUpdate() 
    {
        OnGroundLogic();
        LedgeLogic();
        UseCustomGravity();

        rb.velocity = RaposUtil.AlignWithTransform(transform, new Vector2 (horizontalSpeed, verticalSpeed));
    }

    private void OnGroundLogic()
    {
        onGround = checkGround.OnGround;

        if (onGround && verticalSpeed < 0)
        {
            Vector3 projection = Vector3.Project(rb.velocity, transform.up);
            if (Mathf.Abs(projection.y) <= .001f)
                verticalSpeed = 0;
        }
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
        (bool valid, GravityArea area, float multiplier) gravity = gravityInteraction.GetGravityArea();
        if (!gravity.valid)
            return;

        verticalSpeed += Physics2D.gravity.y * gravity.area.intensity * gravity.multiplier * Time.fixedDeltaTime;
        verticalSpeed = Mathf.Clamp( verticalSpeed, - MAX_GRAVITY, MAX_GRAVITY );
     }
}
