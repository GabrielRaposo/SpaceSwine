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
    [SerializeField] private float speed = 0.02f;
    private Vector2 movDirection;

    [SerializeField] public Transform spritesTransform;
    [SerializeField] ParticleSystem smokeTrailPS;

    private NavigationObject selectedObject;

    public bool lockControls;

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

    private void OnTriggerEnter2D(Collider2D col)
    {
        var navObject = col.gameObject.GetComponent<NavigationObject>();
        if(navObject == null) return;
        
        if(selectedObject != null)
            selectedObject.OnDisselect();
        
        navObject.OnSelect();
        selectedObject = navObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var navObject = other.gameObject.GetComponent<NavigationObject>();
        if(navObject == null)
            return;
        
        navObject.OnDisselect();

        if (navObject == selectedObject)
            selectedObject = null;
    }

    private void FixedUpdate()
    {
        if(lockControls) return;
        
        Vector2 input = movementInputAction.ReadValue<Vector2>();
        movDirection += input * aceleration;
        TrailParticleLogic(activate: input != Vector2.zero);

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

        transform.Translate(movDirection*speed);
        
    }

    private void TrailParticleLogic (bool activate)
    {
        if (!smokeTrailPS)
            return;

        if (activate)
        {
            if (!smokeTrailPS.isPlaying)
                smokeTrailPS.Play();
        }
        else
        {
            if (smokeTrailPS.isPlaying)
                smokeTrailPS.Stop();
        }
    }

    private void ConfirmAction()
    {
        if(selectedObject == null)
            return;

        selectedObject.OnInteract(this);
    }

}
