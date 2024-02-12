﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public static bool ControlsLocked;

    private static Vector2 previousPosition = new Vector2(0f, 330f-1.28f);

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event OnHoverAKEvent;
    [SerializeField] float flightStepsDelay;
    [SerializeField] AK.Wwise.Event flightStepsAKEvent;
    //[SerializeField] AK.Wwise.Event flightAmbienceAKEvent;

    public bool OverrideMode = false;
    public Vector2 OverridenControls = Vector2.zero; // -- Macetagem pra manter o som ativo no auto-pilot

    NavigationShipSoundController movementSoundController; 
    Coroutine flightStepsRoutine;

    bool playingFlightStepsSound;

    private void Awake() 
    {
        movementSoundController = GetComponent<NavigationShipSoundController>();    
    }

    private void OnEnable()
    {
        movDirection = Vector2.zero; 

        transform.position = previousPosition;
        
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Movement.Enable();
        _playerInputActions.Player.Jump.Enable();
        _playerInputActions.Player.Interact.Enable();

        movementInputAction = _playerInputActions.Player.Movement;
        _playerInputActions.Player.Jump.performed += ctx => { ConfirmAction();};
        _playerInputActions.Player.Interact.performed += ctx => { ConfirmAction();};

        ControlsLocked = false;
    }

    private void OnDisable()
    {
        movDirection = Vector2.zero;

        SavePreviousPosition();

        _playerInputActions.Player.Movement.Disable();
        _playerInputActions.Player.Jump.Disable();
        _playerInputActions.Player.Interact.Disable();

        ControlsLocked = true;

        if (!SaveManager.Initiated)
            return;
        
        SaveManager.SetNavigationData(transform.position, spritesTransform ? spritesTransform.eulerAngles.z : 0);
    }

    private void Start() 
    {
        if (!SaveManager.Initiated)
            return;

        var data = SaveManager.GetNavigationData();
        if (!data.initiated)
            return;

        transform.position = data.position;
        if (spritesTransform)
            spritesTransform.eulerAngles = Vector3.forward * data.angle;
    }

    public void LockControls()
    {
        ControlsLocked = true;
        enabled = false;
    }

    public void UnlockControls()
    {
        ControlsLocked = false;
        enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var navObject = col.gameObject.GetComponent<NavigationObject>();
        if(navObject == null) return;
        
        if(selectedObject != null)
            selectedObject.OnDeselect();

        if (OnHoverAKEvent != null)
            OnHoverAKEvent.Post(gameObject);

        navObject.OnSelect();
        selectedObject = navObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var navObject = other.gameObject.GetComponent<NavigationObject>();
        if (navObject == null)
            return;
        
        navObject.OnDeselect();

        if (navObject == selectedObject)
            selectedObject = null;
    }

    private void FixedUpdate()
    {
        if (OverrideMode)
        {
            if (OverridenControls != Vector2.zero)
            {
                if (movementSoundController)
                    movementSoundController.ReadInput(OverridenControls.normalized, intensity: 1.84f);
            }
            else
            {
                if (movementSoundController)
                    movementSoundController.ReadInput(Vector2.zero, intensity: 0f);
            }

            return;
        }
    
        SavePreviousPosition();
        if (ControlsLocked) return;
        
        Vector2 input = movementInputAction.ReadValue<Vector2>();
        movDirection += input * aceleration;
        if (movementSoundController)
            movementSoundController.ReadInput(input.normalized, movDirection.magnitude);
        TrailParticleLogic(activate: input != Vector2.zero);

        if (movDirection.magnitude > movmentCap)
            movDirection = movDirection.normalized * movmentCap;
                

        if (input.magnitude < 0.001f)
        {
            movDirection *= deceleration;
            if (movDirection.magnitude < 0.01f)
                movDirection = Vector2.zero;
        }
        else
        {
            spritesTransform.eulerAngles = new Vector3(0, 0, Mathg.VectorToAngle(movDirection, true));    
        }

        if (NavigationParalaxAnchor.Instance)
            NavigationParalaxAnchor.Instance.transform.Translate(movDirection*speed*20f);
        
        transform.Translate (movDirection * speed);
    }


    private void TrailParticleLogic (bool activate)
    {
        if (!smokeTrailPS)
            return;

        if (activate)
        {
            if (!smokeTrailPS.isPlaying)
                smokeTrailPS.Play();

            SetFlightStepsSound (true);
        }
        else
        {
            if (smokeTrailPS.isPlaying)
                smokeTrailPS.Stop();

            SetFlightStepsSound (false);
        }
    }

    public void SetFlightStepsSound (bool value)
    {
        if (value)
        {
            if (!playingFlightStepsSound)
            { 
                if (flightStepsRoutine != null)
                    StopAllCoroutines();

                flightStepsRoutine = StartCoroutine( FlightStepsLoop() );
                
                playingFlightStepsSound = true;
            }
        }
        else
        {
            if (flightStepsRoutine != null)
                StopAllCoroutines();

            playingFlightStepsSound = false;
        }
    }

    private IEnumerator FlightStepsLoop()
    {
        if (flightStepsAKEvent == null)
            yield break;

        while (true) 
        {
            yield return new WaitForSeconds (flightStepsDelay);
            //Debug.Log("step");
            
            flightStepsAKEvent.Post(gameObject);
        }
    }

    private void ConfirmAction()
    {
        if (OverrideMode || selectedObject == null)
            return;

        selectedObject.OnInteract(this);
    }

    public void SavePreviousPosition()
    {
        previousPosition = transform.position;
    }
}