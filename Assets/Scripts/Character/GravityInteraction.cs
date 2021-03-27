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
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
    }

    private void FixedUpdate() 
    {
        if (!gravityArea)
            return;

        Vector2 direction = (transform.position - gravityArea.Center).normalized;
        float angle = Vector2.SignedAngle(Vector2.up, direction);

        transform.eulerAngles = Vector3.forward * angle;
        rb.SetRotation(angle);
    }

    private void OnTriggerEnter2D(Collider2D collision) 
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
