﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CustomDevCameraController : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float moveSpeed;
    [SerializeField] float zoomSpeed;
    [SerializeField] float tweenDuration;

    [Header("References")]
    [SerializeField] Transform camT;
    [SerializeField] CameraSizeController cameraSizeController;
    [SerializeField] CameraFocusController cameraFocusController;
    [SerializeField] GameObject screenSpaceCollider;

    float customSize = 4.0f;

    PlayerInputActions inputActions;
    InputAction movementInput;
    InputAction input1;
    InputAction input2;

    private void OnEnable() 
    {
        if (!camT || !cameraSizeController || !cameraFocusController) 
        {
            enabled = false;
            return;
        }

        customSize = CameraSizeController.Size;

        inputActions = new PlayerInputActions();

        movementInput = inputActions.Dev.Movement;
        movementInput.Enable();

        input1 = inputActions.Dev.Input1;
        input2 = inputActions.Dev.Input2;

        input1.Enable();
        input2.Enable();

        inputActions.Dev.Input3.performed += (ctx) => CustomTween();
        inputActions.Dev.Input3.Enable();

        inputActions.Enable();

        if (screenSpaceCollider)
            screenSpaceCollider.SetActive(false);
    }

    private void Update() 
    {
        Vector3 direction = movementInput.ReadValue<Vector2>();
        if (direction != Vector3.zero)
            camT.localPosition += direction * moveSpeed * Time.deltaTime;

        if (input1.ReadValue<float>() > .5f)
            SetCameraSize(+1);
        else if (input2.ReadValue<float>() > .5f)
            SetCameraSize(-1);
    }

    private void SetCameraSize (int direction)
    {
        customSize += zoomSpeed * direction * Time.fixedDeltaTime; 
        CameraSizeController.Size = customSize;
    }

    private void CustomTween()
    {
        float fromSize = customSize;
        float toSize = 4.0f;

        Vector3 fromPos = camT.localPosition;
        Vector3 toPos = new Vector3(0, 0, camT.localPosition.z);

        Sequence s = DOTween.Sequence();

        s.Append
        ( 
            DOVirtual.Float(fromSize, toSize, tweenDuration, (f) => CameraSizeController.Size = f )
        );
        s.Join
        ( 
            DOVirtual.Float(0, 1, tweenDuration, (t) => 
            {
                Vector3 value = Vector3.Lerp(fromPos, toPos, t);
                camT.localPosition = value;
            })
        );
    }

    private void OnDisable() 
    {
        if (!camT || !cameraSizeController || !cameraFocusController)
            return;

        movementInput.Disable();

        input1.Disable();
        input2.Disable();
        inputActions.Dev.Input3.Disable();

        inputActions.Disable();    

        //if (screenSpaceCollider)
        //    screenSpaceCollider.SetActive(true);
    }
}
