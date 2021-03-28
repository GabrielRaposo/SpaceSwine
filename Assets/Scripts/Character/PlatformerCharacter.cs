using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GravityInteraction))]
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

    float horizontalSpeed;
    float verticalSpeed;

    GravityInteraction gravityInteraction;
    CheckGround checkGround;
    CheckWall checkWall;
    Rigidbody2D rb;

    void Awake()
    {
        gravityInteraction = GetComponent<GravityInteraction>();
        checkGround = GetComponent<CheckGround>();
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
        if (!checkGround.OnGround)
            UseCustomGravity();

        //rb.velocity = (horizontalMovement * transform.right) + (verticalVelocity * transform.up);
        rb.velocity = RaposUtil.AllignVectorWithTransform(transform, new Vector2 (horizontalSpeed, verticalSpeed));
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
