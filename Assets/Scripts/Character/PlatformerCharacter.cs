using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacter : SidewaysCharacter
{
    const float MAX_GRAVITY = 9.8f;

    [Header("Values")]
    [SerializeField] float speed;
    [SerializeField] float jumpForce;

    [Header("References")]
    [SerializeField] SpriteRenderer directionArrow;

    float horizontalMovement;
    float verticalVelocity;

    GravityInteraction gravityInteraction;
    CheckGround checkGround;
    Rigidbody2D rb;

    void Awake()
    {
        gravityInteraction = GetComponent<GravityInteraction>();
        checkGround = GetComponent<CheckGround>();
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
        horizontalMovement = horizontalInput * speed;
    }

    public void JumpInput()
    {
        if (!checkGround.OnGround)
            return;

        verticalVelocity = jumpForce;
    }

    private void FixedUpdate() 
    {
        if (!checkGround.OnGround)
            UseCustomGravity();

        rb.velocity = (horizontalMovement * transform.right) + (verticalVelocity * transform.up);
    }

    private void UseCustomGravity()
    {
        (bool valid, GravityArea area, float multiplier) gravity = gravityInteraction.GetGravityArea();
        if (!gravity.valid)
            return;

        verticalVelocity += Physics2D.gravity.y * gravity.area.intensity * gravity.multiplier * Time.fixedDeltaTime;
        verticalVelocity = Mathf.Clamp( verticalVelocity, - MAX_GRAVITY, MAX_GRAVITY );
     }
}
