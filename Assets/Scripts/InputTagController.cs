using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerType { Keyboard, XboxJoystick }

public static class InputTagController
{
    public static ControllerType CurrentControllerType = ControllerType.Keyboard;

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

    public static string Launch
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
