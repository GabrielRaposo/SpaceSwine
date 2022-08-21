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

    public static bool ControlsLocked;

    private static Vector2 previousPostion = new Vector2(0f, 330f-1.28f);

    [Header("Audio")]
    [SerializeField] float flightStepsDelay;
    [SerializeField] AK.Wwise.Event flightStepsAKEvent;
    //[SerializeField] AK.Wwise.Event flightAmbienceAKEvent;

    NavigationShipSoundController movementSoundController; 
    Coroutine flightStepsRoutine;

    private void Awake() 
    {
        movementSoundController = GetComponent<NavigationShipSoundController>();    
    }

    private void OnEnable()
    {
        transform.position = previousPostion;
        
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Movement.Enable();
        _playerInputActions.Player.Jump.Enable();
        _playerInputActions.Player.Throw.Enable();

        movementInputAction = _playerInputActions.Player.Movement;
        _playerInputActions.Player.Jump.performed += ctx => { ConfirmAction();};
        _playerInputActions.Player.Throw.performed += ctx => { ConfirmAction();};

        ControlsLocked = false;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Movement.Disable();
        _playerInputActions.Player.Jump.Disable();
        _playerInputActions.Player.Throw.Disable();

        ControlsLocked = true;
    }

    public void LockControls()
    {
        ControlsLocked = true;
        OnDisable();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var navObject = col.gameObject.GetComponent<NavigationObject>();
        if(navObject == null) return;
        
        if(selectedObject != null)
            selectedObject.OnDeselect();
        
        navObject.OnSelect();
        selectedObject = navObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var navObject = other.gameObject.GetComponent<NavigationObject>();
        if(navObject == null)
            return;
        
        navObject.OnDeselect();

        if (navObject == selectedObject)
            selectedObject = null;
    }

    private void FixedUpdate()
    {
        previousPostion = transform.position;
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
            if(movDirection.magnitude<0.01f)
                movDirection = Vector2.zero;
        }
        else
        {
            spritesTransform.eulerAngles = new Vector3(0, 0, Mathg.VectorToAngle(movDirection, true));    
        }

        if(NavigationParalaxAnchor.Instance)
            NavigationParalaxAnchor.Instance.transform.Translate(movDirection*speed*20f);
        
        transform.Translate(movDirection * speed);
    }

    private void TrailParticleLogic (bool activate)
    {
        if (!smokeTrailPS)
            return;

        if (activate)
        {
            if (!smokeTrailPS.isPlaying)
            {
                smokeTrailPS.Play();

                if (flightStepsRoutine != null)
                    StopAllCoroutines();

                flightStepsRoutine = StartCoroutine( FlightStepsLoop() );
            }

            //if (flightAmbienceAKEvent != null && !flightAmbienceAKEvent.IsPlaying(gameObject))
            //    flightAmbienceAKEvent.Post(gameObject);
        }
        else
        {
            if (smokeTrailPS.isPlaying)
            {
                smokeTrailPS.Stop();
                
                if (flightStepsRoutine != null)
                    StopAllCoroutines();
            }


            //if (flightAmbienceAKEvent != null && flightAmbienceAKEvent.IsPlaying(gameObject))
            //    flightAmbienceAKEvent.Stop(gameObject);
        }
    }

    private IEnumerator FlightStepsLoop()
    {
        if (flightStepsAKEvent == null)
            yield break;

        while (true) 
        {
            yield return new WaitForSeconds(flightStepsDelay);
            //Debug.Log("step");
            
            //flightStepsAKEvent.Post(gameObject);
        }
    }

    private void ConfirmAction()
    {
        if(selectedObject == null)
            return;

        selectedObject.OnInteract(this);
    }

}
