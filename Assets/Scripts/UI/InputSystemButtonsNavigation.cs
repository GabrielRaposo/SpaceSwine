using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.Serialization;

public abstract class InputSystemButtonsNavigation<ButtonType> : MonoBehaviour where ButtonType : InputSystemCompatibleButton
{
    [SerializeField] protected bool startOnFocus;
    [FormerlySerializedAs("titleButtons")] [SerializeField] protected List<ButtonType> buttons;
    
    [Header("Input")]
    [SerializeField] protected float holdCooldown;
    [HideInInspector] public bool OnFocus;

    protected PlayerInputActions playerInputActions;
    protected InputAction axisInput;

    protected int current = -1;
    protected float holdCount;
    
    protected virtual void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();    
    
        axisInput = playerInputActions.UI.Navigation;

        axisInput.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus)
                return;

            buttons[current].Submit();
        };
        playerInputActions.UI.Confirm.Enable();
    }
    
    protected virtual void Start() 
    {
        if (startOnFocus)
            OnFocus = true;
    }
    
    private void Update() 
    {
        Vector2 axis = axisInput.ReadValue<Vector2>();
        if (axis == Vector2.zero)
            holdCount = 0;
        else
            holdCount -= Time.deltaTime;

        if (holdCount < 0)
            holdCount = 0;

        if (!OnFocus || SceneTransition.OnTransition)
            return;
        
        if (holdCount > 0)
            return;

        if (axis.y != 0)
        {
            if (axis.y > .75f)
                MoveCursor(-1);
            else if (axis.y < -.75f)
                MoveCursor(1);
        }
    }
    
    protected virtual void MoveCursor (int direction)
    {
        holdCount = holdCooldown;
        int buttonCount = ActiveButtonsCount();

        current += direction;
        // if (current % buttonCount == 0)
        //     current += direction;

        if (current < 0)
            current = buttonCount - 1;
        
        current %= buttonCount;
        
        Debug.Log($"Current: {current}");

        SelectCurrent(instant: false, playSound: true);
    }

    protected void SelectCurrent (bool instant = false, bool playSound = false)
    {
        //Debug.Log("current: " + current);

        for (int i = 0; i < buttons.Count; i++)
        {
            if (instant)
                buttons[i].InstantSelect(false);
            else
                buttons[i].Deselect();
        }

        if (instant)
            buttons[current].InstantSelect(true);
        else
            buttons[current].Select(playSound);        
    }
    
    private void OnDisable() 
    {
        axisInput.Disable();
        playerInputActions.UI.Confirm.Disable();
    }

    protected int ActiveButtonsCount()
    {
        int n = 0;
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].gameObject.activeSelf)
                n++;

        }

        return n;
    }

}
