using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GGSConsoleButtonsController : MonoBehaviour
{
    [SerializeField] ImageSwapper actionButtonSwapper;
    [SerializeField] ImageSwapper upButtonSwapper;
    [SerializeField] ImageSwapper downButtonSwapper;
    [SerializeField] ImageSwapper leftButtonSwapper;
    [SerializeField] ImageSwapper rightButtonSwapper;

    PlayerInputActions playerInput;
    InputAction movement;
    InputAction action;

    void OnEnable()
    {
        playerInput = new PlayerInputActions();

        movement = playerInput.Player.Movement;
        movement.Enable();

        action = playerInput.Player.Jump;
        action.Enable();
    }

    private void Update() 
    {
        actionButtonSwapper.SetSpriteState( action.ReadValue<float>() > .5f ? 1 : 0 );

        Vector2 movementInput = movement.ReadValue<Vector2>();
        
        upButtonSwapper   .SetSpriteState( movementInput.y > 0 ? 1 : 0 );
        downButtonSwapper .SetSpriteState( movementInput.y < 0 ? 1 : 0 );
        leftButtonSwapper .SetSpriteState( movementInput.x < 0 ? 1 : 0 );
        rightButtonSwapper.SetSpriteState( movementInput.x > 0 ? 1 : 0 );
    }

    private void OnDisable() 
    {
        movement.Disable();
        action.Disable();
    }
}
