using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Cinemachine;

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
    [SerializeField] Animator eyeAnimator1;
    [SerializeField] Animator eyeAnimator2;

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

        inputActions = new PlayerInputActions();

        movementInput = inputActions.Dev.Movement;
        movementInput.Enable();

        input1 = inputActions.Dev.Input1;
        input2 = inputActions.Dev.Input2;

        input1.Enable();
        input2.Enable();

        inputActions.Dev.Input3.performed += (ctx) => CustomTween2();
        inputActions.Dev.Input3.Enable();

        inputActions.Enable();

        if (screenSpaceCollider)
            screenSpaceCollider.SetActive(false);
    }


    private void Start() 
    {
        customSize = CameraSizeController.Size;    
    }

    private void Update() 
    {
        Vector3 direction = movementInput.ReadValue<Vector2>();
        if (direction != Vector3.zero)
        {
            CinemachineVirtualCamera vcam = camT.GetComponentInParent<CinemachineVirtualCamera>();
            if (vcam)
            {
                vcam.Follow = camT;
                camT.SetParent(null);
            }

            camT.localPosition += direction * moveSpeed * Time.deltaTime;
        }

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

    private void CustomTween1()
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

    private void CustomTween2()
    {
        float fromSize = 2.0f;
        float toSize = 3.5f;

        //Vector3 fromPos = camT.localPosition;
        //Vector3 toPos = new Vector3(0, 0, camT.localPosition.z);

        Sequence s = DOTween.Sequence();

        s.Append
        ( 
            DOVirtual.Float(fromSize, toSize, tweenDuration, (f) => CameraSizeController.Size = f )
                .SetEase(Ease.Linear)
        );
        
        RaposUtil.WaitSeconds(this, 3.0f, () => 
        {
            if (eyeAnimator1)
                eyeAnimator1.enabled = true;
        });
        RaposUtil.WaitSeconds(this, 3.0f, () => 
        {
            if (eyeAnimator2)
                eyeAnimator2.enabled = true;
        });

        //s.Join
        //( 
        //    DOVirtual.Float(0, 1, tweenDuration, (t) => 
        //    {
        //        Vector3 value = Vector3.Lerp(fromPos, toPos, t);
        //        camT.localPosition = value;
        //    })
        //);
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
