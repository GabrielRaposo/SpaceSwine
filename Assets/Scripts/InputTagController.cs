using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ControllerType { Keyboard, XboxJoystick }

public static class InputTagController
{
    public static UnityAction OnInputTypeChanged;

    public static ControllerType CurrentControllerType = ControllerType.XboxJoystick;

    public static void SetControllerType(ControllerType controllerType)
    {
        if (OnInputTypeChanged != null)
            OnInputTypeChanged.Invoke();

        CurrentControllerType = controllerType;
    }

    public static string GetInput(string tag)
    {
        switch (tag)
        {
            case "movement": 
                return Movement;

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
                case ControllerType.Keyboard:       return "Arrows";
                case ControllerType.XboxJoystick:   return "Left Stick";
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
                case ControllerType.Keyboard:       return "Space Bar";
                case ControllerType.XboxJoystick:   return "(A)";
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
                case ControllerType.Keyboard:       return "X";
                case ControllerType.XboxJoystick:   return "(X)";
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
                case ControllerType.Keyboard:       return "C";
                case ControllerType.XboxJoystick:   return "(Y)";
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
                case ControllerType.Keyboard:       return "Escape";
                case ControllerType.XboxJoystick:   return "(Start)";
            }
        }
    }
}
