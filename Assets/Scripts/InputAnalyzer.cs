using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputAnalyzer : MonoBehaviour
{
    PlayerInputActions inputActions;

    void OnEnable()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Jump.performed += AnalyzeInput;
        inputActions.Player.Movement.performed += AnalyzeInput;

        inputActions.Player.Jump.Enable();
        inputActions.Enable();

    }

    private void AnalyzeInput (InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is Keyboard)
        {
            var binding = ctx.action.GetBindingForControl(ctx.control);
            var binding2 = ctx.action.GetBindingIndexForControl(ctx.control);
            InputTagController.ReadKeyboardBindings (binding + " " + binding2);

            if (InputTagController.CurrentControllerType != ControllerType.Keyboard)
                InputTagController.SetControllerType( ControllerType.Keyboard );
        }
            
        if (ctx.control.device is Gamepad)
        {
            if (InputTagController.CurrentControllerType != ControllerType.XboxJoystick)
                InputTagController.SetControllerType( ControllerType.XboxJoystick );
        }
    }

    private void OnDisable() 
    {
        inputActions.Player.Jump.Disable();
        inputActions.Disable();
    }
}
