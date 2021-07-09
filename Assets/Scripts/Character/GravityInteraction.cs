using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public UnityAction<Transform> OnChangeGravityAnchor; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
        checkGround = GetComponent<CheckGround>();
    }

    private void Update() 
    {
        CameraFocusController cameraFocusController = CameraFocusController.Instance;

        if (Input.GetKeyDown(KeyCode.O))
            cameraFocusController.SetPlayerFocus();
        if (Input.GetKeyDown(KeyCode.P) && gravityArea)
            cameraFocusController.SetPlanetFocus(gravityArea.transform);
    }

    private void FixedUpdate() 
    {
        if (checkGround)
            platform = checkGround.OnPlatform;

        UpdateParent ();

        float angle = 0;
        if (!platform)
        {
            if (!gravityArea)
                return;

            angle = AlignWithPlanet();
        }
        else
        {
            angle = AlignWithPlatform();
        }

        float interpolatedAngle = angle;
        if (!lockIntoAngle)
        {
            if (transform.eulerAngles.z == angle)
                return;

            int direction = RaposUtil.AngleDifference(transform.eulerAngles.z, angle) > 0 ? 1 : -1;
            interpolatedAngle = transform.eulerAngles.z + (angleAdjustment * direction * Time.fixedDeltaTime);

            if (Mathf.Abs(interpolatedAngle - angle) < 1f)
                interpolatedAngle = angle;
        }

        transform.eulerAngles = Vector3.forward * interpolatedAngle;
        rb.SetRotation(interpolatedAngle);
    }

    private void UpdateParent ()
    {
        if (platform)
        {
            if (transform.parent == platform)
                return;

            transform.SetParent (platform.transform);
            //OnChangeGravityAnchor?.Invoke(platform.transform);
        }
        else
        {
            if (transform.parent == null)
                return;

            transform.SetParent (null);
            transform.localScale = Vector3.one;
            //OnChangeGravityAnchor?.Invoke(null);
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
        OnChangeGravityAnchor?.Invoke(gravityArea.transform);
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (!this.gravityArea)
            return;

        GravityArea gravityArea = collision.GetComponent<GravityArea>();
        if (this.gravityArea == gravityArea)
        {
            this.gravityArea = null;
            OnChangeGravityAnchor?.Invoke(null);
        }
    }

    public (bool, GravityArea, float multiplier, bool onPlatform) GetGravityArea()
    {
        return (gravityArea != null, gravityArea, jumpHeld ? lowGravityMultiplier : defaultMultiplier, platform); 
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
