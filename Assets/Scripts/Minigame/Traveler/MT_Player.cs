using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MT_Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform aimAnchor;

    Vector2 direction;
    Rigidbody2D rb;
    MT_ScreenLooper screenLooper;

    PlayerInputActions inputActions;
    InputAction movementAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        screenLooper = GetComponent<MT_ScreenLooper>();
    }

    private void OnEnable()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        movementAction = inputActions.Minigame.Movement;
        movementAction.Enable();

        inputActions.Minigame.Action.performed += (ctx) => 
        {
            ShootInput();
        };
        inputActions.Minigame.Action.Enable();
    }

    void Start()
    {
        direction = Vector2.up;
        UpdateAimDisplay();
    }

    void UpdateAimDisplay()
    {
        if (!aimAnchor || !screenLooper.InsideZone)
            return;

        aimAnchor.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
    }

    void ShootInput()
    {
        rb.velocity = speed * direction * -1;
    }

    void Update()
    {
        Vector2 axisInput = movementAction.ReadValue<Vector2>();
        
        if (axisInput == Vector2.zero)
            return;

        direction = axisInput.normalized;
        UpdateAimDisplay();
    }

    private void OnDisable()
    {
        inputActions.Disable();

        movementAction.Disable();
        inputActions.Minigame.Action.Disable();
    }
}
