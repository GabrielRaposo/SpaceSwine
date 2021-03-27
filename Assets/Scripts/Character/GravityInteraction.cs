using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityInteraction : MonoBehaviour
{
    [SerializeField] float defaultMultiplier = 1.0f;
    [SerializeField] float lowGravityMultiplier = .8f; 

    bool jumpHeld;

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

        float angle = 0;

        UpdateParent ();

        if (!platform)
            angle = AlignWithPlanet();
        else
            angle = AlignWithPlatform();

        transform.eulerAngles = Vector3.forward * angle;
        rb.SetRotation(angle);
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

}
