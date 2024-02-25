using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ControllerType { Keyboard, XboxJoystick }
public enum KeyboardScheme { Arrows, WASD }


public static class InputTagController
{
    public static UnityAction OnInputTypeChanged;

    public static ControllerType CurrentControllerType = ControllerType.Keyboard;
    public static KeyboardScheme CurrentKeyboardScheme = KeyboardScheme.Arrows;

    public static void SetControllerType(ControllerType controllerType)
    {
        CurrentControllerType = controllerType;
        
        if (OnInputTypeChanged != null)
            OnInputTypeChanged.Invoke();
    }

    public static void SetKeyboardScheme(KeyboardScheme keyboardScheme) 
    {
        CurrentKeyboardScheme = keyboardScheme;
        //Debug.Log("CurrentKeyboardScheme: " + CurrentKeyboardScheme);

        if (OnInputTypeChanged != null)
            OnInputTypeChanged.Invoke();
    }

    public static void ReadKeyboardBindings(string bindingsInfo)
    {
        DebugDisplay.Call(bindingsInfo);

        if (bindingsInfo.Contains("Movement:2DVector"))
        {
            if (int.TryParse( bindingsInfo.Substring(bindingsInfo.Length - 2), out int result))
            {
                if (result == 0)
                {
                    if (CurrentKeyboardScheme != KeyboardScheme.Arrows)
                        SetKeyboardScheme (KeyboardScheme.Arrows);
                }
                else 
                {
                    if (CurrentKeyboardScheme != KeyboardScheme.WASD)
                        SetKeyboardScheme (KeyboardScheme.WASD);
                }
            }
        }
        else if (bindingsInfo.Contains("Jump:<Keyboard>/"))
        {
            if (int.TryParse( bindingsInfo.Substring(bindingsInfo.Length - 2), out int result))
            {
                if (result == 0 || result == 2)
                {
                    if (CurrentKeyboardScheme != KeyboardScheme.Arrows)
                        SetKeyboardScheme (KeyboardScheme.Arrows);
                }
                else 
                {
                    if (CurrentKeyboardScheme != KeyboardScheme.WASD)
                        SetKeyboardScheme (KeyboardScheme.WASD);
                }
            }
        }
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
                case ControllerType.Keyboard:
                    if (CurrentKeyboardScheme == KeyboardScheme.Arrows)
                        return LocalizationManager.GetInputText("movement_keyboard");
                    else 
                        return LocalizationManager.GetInputText("movement_keyboard2");

                case ControllerType.XboxJoystick:   
                    return LocalizationManager.GetInputText("movement_joystick");
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
                case ControllerType.Keyboard:
                    if (CurrentKeyboardScheme == KeyboardScheme.Arrows)
                        return LocalizationManager.GetInputText("movement2_keyboard");
                    else 
                        return LocalizationManager.GetInputText("movement2_keyboard2");

                case ControllerType.XboxJoystick:   
                    return LocalizationManager.GetInputText("movement2_joystick");
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
                case ControllerType.Keyboard:       
                    if (CurrentKeyboardScheme == KeyboardScheme.Arrows)
                        return LocalizationManager.GetInputText("jump_keyboard");
                    else 
                        return LocalizationManager.GetInputText("jump_keyboard2");

                case ControllerType.XboxJoystick:   
                    return LocalizationManager.GetInputText("jump_joystick");
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
                case ControllerType.Keyboard:       
                    if (CurrentKeyboardScheme == KeyboardScheme.Arrows)
                        return LocalizationManager.GetInputText("throw_keyboard");
                    else 
                        return LocalizationManager.GetInputText("throw_keyboard2");

                case ControllerType.XboxJoystick:   
                    return LocalizationManager.GetInputText("throw_joystick");
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
                case ControllerType.Keyboard:       
                    if (CurrentKeyboardScheme == KeyboardScheme.Arrows)
                        return LocalizationManager.GetInputText("interact_keyboard");
                    else 
                        return LocalizationManager.GetInputText("interact_keyboard2");

                case ControllerType.XboxJoystick:   
                    return LocalizationManager.GetInputText("interact_joystick");
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
