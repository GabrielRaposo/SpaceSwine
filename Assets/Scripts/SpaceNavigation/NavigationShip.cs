using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavigationShip : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    private InputAction movementInputAction;

    private float deceleration = 0.95f;
    private float aceleration = 0.04f;
    private float movmentCap=1.8f;
    [SerializeField]private float speed = 0.02f;
    private Vector2 movDirection;

    public Transform debugInput;
    public Transform debugMov;

    [SerializeField] private Transform spritesTransform;

    private void OnEnable()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Movement.Enable();
        _playerInputActions.Player.Jump.Enable();
        _playerInputActions.Player.Throw.Enable();

        movementInputAction = _playerInputActions.Player.Movement;
        _playerInputActions.Player.Jump.performed += ctx => { ConfirmAction();};
        _playerInputActions.Player.Throw.performed += ctx => { ConfirmAction();};
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Movement.Disable();
        _playerInputActions.Player.Jump.Disable();
        _playerInputActions.Player.Throw.Disable();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 input = movementInputAction.ReadValue<Vector2>();
        debugInput.localPosition = input;
        
        movDirection += input * aceleration;

        if (movDirection.magnitude > movmentCap)
            movDirection = movDirection.normalized * movmentCap;
                

        if (input.magnitude < 0.001f)
        {
            movDirection *= deceleration;
            if(movDirection.magnitude<0.01f)
                movDirection = Vector2.zero;
        }
        else
        {
            spritesTransform.eulerAngles = new Vector3(0, 0, Mathg.VectorToAngle(movDirection, true));    
        }

        debugMov.localPosition = movDirection;
            
        transform.Translate(movDirection*speed);
        
    }

    private void ConfirmAction()
    {
        Debug.Log("bip");
    }
    
}
