using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityInteraction : MonoBehaviour
{
    [SerializeField] float defaultMultiplier = 1.0f;
    [SerializeField] float lowGravityMultiplier = .8f; 
    [SerializeField] float angleAdjustment;

    [Space(5)]

    [SerializeField] InputAction playerFocusInput;
    [SerializeField] InputAction planetFocusInput;

    bool jumpHeld;
    bool lockIntoAngle;

    Rigidbody2D rb;
    CheckGround checkGround;

    GravityArea gravityArea;
    PlanetPlatform platform;

    GravitationalBody gravitationalBody;
    GravitationalBody overrideGravitationalBody; 
    // -- Usado para fazer a transição entre Planetas e Closed Spaces

    public GravitationalBody GBody
    {
        get 
        {
            if (overrideGravitationalBody != null)
                return overrideGravitationalBody;

            return gravitationalBody;
        }
    }

    public GravitationalBody SetOverrideGravitationalBody
    {
        set 
        {
            if (value == null)
            {
                gravityArea = null;

                // -- "Pisca" o collider para atualizar a gravityArea
                Collider2D coll = GetComponentInChildren<Collider2D>();
                if (coll)
                {
                    coll.enabled = false;
                    coll.enabled = true;
                }
            }
            overrideGravitationalBody = value;
        }
    }

    public UnityAction<Transform> OnChangeGravityAnchor;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();   
        checkGround = GetComponent<CheckGround>();
    }

    private void OnEnable() 
    {
        platform = null;
        gravitationalBody = null;
        overrideGravitationalBody = null;

        playerFocusInput.Enable();
        planetFocusInput.Enable();
    }

    void Start()
    {
        CameraFocusController cameraFocusController = CameraFocusController.Instance;

        playerFocusInput.performed += (ctx) => 
        {
            cameraFocusController.SetPlayerFocus();
        };

        planetFocusInput.performed += (ctx) => 
        { 
            if (gravityArea) cameraFocusController.SetPlanetFocus(gravityArea.transform);
        };

        LocalGameplayState localGameplayState = GetComponent<LocalGameplayState>();
        if (localGameplayState)
        {
            switch (localGameplayState.state) 
            {
                case GameplayState.Exploration:
                    if (cameraFocusController)
                        cameraFocusController.SetInstantPlayerFocus();
                    break;

                //case GameplayState.Danger:
                //    if (gravityArea) cameraFocusController.SetPlanetFocus(gravityArea.transform);
                //    break;
            }

        }
    }

    private void Update() 
    {
        CameraFocusController cameraFocusController = CameraFocusController.Instance;

        //if (Input.GetKeyDown(KeyCode.O))
        //    cameraFocusController.SetPlayerFocus();
        //if (Input.GetKeyDown(KeyCode.P) && gravityArea)
        //    cameraFocusController.SetPlanetFocus(gravityArea.transform); 
    }

    private void FixedUpdate() 
    {
        if (checkGround)
        {
            platform = checkGround.OnPlatform;

            GravitationalBody gBody = checkGround.OnPlanet;
            if (overrideGravitationalBody == null)
            {
                gravitationalBody = gBody;
            }
        }

        UpdateParent ();

        float angle = 0;

        if (platform)
        {
            angle = AlignWithPlatform();
        } 
        else if (GBody)
        {
            angle = AlignWithPlanet();
        }
        else if (gravityArea)
        {
            angle = AlignWithPlanet();
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
            if (transform.parent == platform.transform)
                return;

            transform.SetParent (platform.transform);
        }
        else if (GBody)
        {
            if (transform.parent == GBody.transform)
                return;

            transform.SetParent (GBody.transform);
            GBody.OnLandAction?.Invoke(transform);
        }
        else
        {
            if (transform.parent == null)
                return;

            transform.SetParent (null);
            transform.localScale = Vector3.one;
        }
    }

    public float AlignWithPlanet()
    {
        if (!gravityArea)
            return 0;

        if (gravityArea.linear)
            return Vector2.SignedAngle(Vector2.up, gravityArea.transform.up); 

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

    public (bool isValid, GravityArea Area, float multiplier, bool onPlatform) GetGravityArea()
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

    public void DettachFromSurfaces()
    {
        platform = null;
        gravitationalBody = null;
        overrideGravitationalBody = null;
        UpdateParent();
    }

    private void OnDisable() 
    {
        platform = null;
        gravitationalBody = null;
        overrideGravitationalBody = null;
        
        playerFocusInput.Disable();
        planetFocusInput.Disable();
    }
}
