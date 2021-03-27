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

    float targetHorizontal;
    float targetVertical;

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

        targetHorizontal = horizontalInput * speed;

        if (checkWall && checkWall.MovingTowardsWall(targetHorizontal))
        {
            targetHorizontal = 0;
        }
    }

    public void JumpInput()
    {
        if (!checkGround.OnGround)
            return;

        targetVertical = jumpForce;
    }

    private void FixedUpdate() 
    {
        if (!checkGround.OnGround)
            UseCustomGravity();

        //rb.velocity = (horizontalMovement * transform.right) + (verticalVelocity * transform.up);
        rb.velocity = RaposUtil.AllignVectorWithTransform(transform, new Vector2 (targetHorizontal, targetVertical));
    }

    private void UseCustomGravity()
    {
        (bool valid, GravityArea area, float multiplier) gravity = gravityInteraction.GetGravityArea();
        if (!gravity.valid)
            return;

        targetVertical += Physics2D.gravity.y * gravity.area.intensity * gravity.multiplier * Time.fixedDeltaTime;
        targetVertical = Mathf.Clamp( targetVertical, - MAX_GRAVITY, MAX_GRAVITY );
     }
}
