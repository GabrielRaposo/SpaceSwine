using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityInteraction : MonoBehaviour
{
    [SerializeField] float defaultMultiplier = 1.0f;
    [SerializeField] float lowGravityMultiplier = .8f; 
    [SerializeField] float angleAdjustment;

    bool jumpHeld;
    bool lockIntoAngle;

    GravityArea gravityArea;
    PlanetPlatform platform;

    CheckGround checkGround;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
        checkGround = GetComponent<CheckGround>();
    }

    private void FixedUpdate() 
    {
        if (checkGround)
            platform = checkGround.OnPlatform;

        UpdateParent ();

        float angle = 0;
        if (!platform)
            angle = AlignWithPlanet();
        else
            angle = AlignWithPlatform();

        float interpolatedAngle = angle;
        if (!lockIntoAngle)
        {
            if (transform.eulerAngles.z == angle)
                return;

            int direction = GetRotationDirection(transform.eulerAngles.z, angle);
            interpolatedAngle = transform.eulerAngles.z + (angleAdjustment * direction * Time.fixedDeltaTime);

            if (Mathf.Abs(interpolatedAngle - angle) < 1f)
                interpolatedAngle = angle;
        }

        transform.eulerAngles = Vector3.forward * interpolatedAngle;
        rb.SetRotation(interpolatedAngle);
    }

    private int GetRotationDirection(float from, float to)
    {
        //Debug.Log("AngleDifference: " + AngleDifference(from, to));

        double diff = AngleDifference(from, to);
        return (diff > 0) ? 1 : -1;
    }

    public static double AngleDifference (double angle1, double angle2)
    {
        double diff = ( angle2 - angle1 + 180 ) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }

    private void UpdateParent ()
    {
        if (platform)
        {
            transform.SetParent(platform.transform);
        }
        else
        {
            transform.SetParent(null);
            transform.localScale = Vector3.one;
        }
    }

    private float AlignWithPlanet()
    {
        if (!gravityArea)
            return 0;

        Vector2 direction = (transform.position - gravityArea.Center).normalized;
        return Vector2.SignedAngle(Vector2.up, direction);
    }

    private float AlignWithPlatform()
    {
        return Vector2.SignedAngle(Vector2.up, platform.transform.up);
    }

    private void OnTriggerEnter2D (Collider2D collision) 
    {
        GravityArea gravityArea = collision.GetComponent<GravityArea>();
        if (!gravityArea)
            return;

        this.gravityArea = gravityArea;
    }

    public (bool, GravityArea, float multiplier) GetGravityArea()
    {
        return (gravityArea != null, gravityArea, jumpHeld ? lowGravityMultiplier : defaultMultiplier); 
    }

    public void SetJumpHeld(bool value)
    {
        jumpHeld = value;
    }

    public void SetLockIntoAngle(bool value)
    {
        lockIntoAngle = value;
    }

}
