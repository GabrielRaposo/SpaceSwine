using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ControllerType { Keyboard, XboxJoystick }

public static class InputTagController
{
    public static UnityAction OnInputTypeChanged;

    public static ControllerType CurrentControllerType = ControllerType.Keyboard;

    public static void SetControllerType(ControllerType controllerType)
    {
        CurrentControllerType = controllerType;
        
        if (OnInputTypeChanged != null)
            OnInputTypeChanged.Invoke();
    }

    public static string GetInput(string tag)
    {
        switch (tag)
        {
            case "movement": 
                return Movement;
            
            case "movement2": 
                return Movement2;

            case "jump": 
                return Jump;

            case "throw":
                return Throw;

            case "interact":
                return Interact;

            case "pause":
                return Pause;
        }

        return tag;
    }

    public static string Movement
    {
        get 
        {
            switch (CurrentControllerType)
            {
                default:
                case ControllerType.Keyboard: return LocalizationManager.GetInputText("movement_keyboard");
                case ControllerType.XboxJoystick:   return LocalizationManager.GetInputText("movement_joystick");
            }
        }
    }

    public static string Movement2
    {
        get 
        {
            switch (CurrentControllerType)
            {
                default:
                case ControllerType.Keyboard:       return LocalizationManager.GetInputText("movement2_keyboard");
                case ControllerType.XboxJoystick:   return LocalizationManager.GetInputText("movement2_joystick");
            }
        }
    }

    public static string Jump
    {
        get 
        {
            switch (CurrentControllerType)
            {
                default:
                case ControllerType.Keyboard:       return LocalizationManager.GetInputText("jump_keyboard");
                case ControllerType.XboxJoystick:   return LocalizationManager.GetInputText("jump_joystick");
            }
        }
    }

    public static string Throw
    {
        get 
        {
            switch (CurrentControllerType)
            {
                default:
                case ControllerType.Keyboard:       return LocalizationManager.GetInputText("throw_keyboard");
                case ControllerType.XboxJoystick:   return LocalizationManager.GetInputText("throw_joystick");
            }
        }
    }

    public static string Interact
    {
        get 
        {
            switch (CurrentControllerType)
            {
                default:
                case ControllerType.Keyboard:       return LocalizationManager.GetInputText("interact_keyboard");
                case ControllerType.XboxJoystick:   return LocalizationManager.GetInputText("interact_joystick");
            }
        }
    }

    public static string Pause
    {
        get 
        {
            switch (CurrentControllerType)
            {
                default:
                case ControllerType.Keyboard:       return LocalizationManager.GetInputText("pause_keyboard");
                case ControllerType.XboxJoystick:   return LocalizationManager.GetInputText("pause_joystick");
            }
        }
    }
}
