using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RestLoopManager : MonoBehaviour
{
    [SerializeField] GameObject restLoopAnimation;
    [SerializeField] PlayerCharacter playerCharacter;

    bool isActive;
    PlayerInputActions playerInputActions;

    public static RestLoopManager Instance;

    void Awake()
    {
        Instance = this;
    }
    private void Start() 
    {
        restLoopAnimation.SetActive(false);
    }

    public void Setup()
    {
        GameManager.BlockCharacterInput = true;
        restLoopAnimation.SetActive(true);
        playerCharacter.SetHiddenState(true);
        TurnControlsOn();
    }

    private void TurnOff()
    {
        GameManager.BlockCharacterInput = false;
        restLoopAnimation.SetActive(false);
        playerCharacter.SetHiddenState(false);
        TurnControlsOff();
    }

    private void TurnControlsOn()
    {
        playerInputActions = new PlayerInputActions();  

        playerInputActions.Player.Jump.performed += OnAnyInput;
        playerInputActions.Player.Jump.Enable();

        playerInputActions.Player.Throw.performed += OnAnyInput;
        playerInputActions.Player.Throw.Enable();

        playerInputActions.Player.Interact.performed += OnAnyInput;
        playerInputActions.Player.Interact.Enable();

        isActive = true;    
    }

    private void TurnControlsOff()
    {
        if (!isActive)
            return;

        playerInputActions.Player.Jump.Disable();
        playerInputActions.Player.Throw.Disable();
        playerInputActions.Player.Interact.Disable();

        isActive = false;
    }

    public void OnAnyInput(InputAction.CallbackContext ctx)
    {
        if (!isActive)
            return;

        TurnOff();
    }

    private void OnDisable() 
    {
        playerCharacter.SetHiddenState(false);
        TurnControlsOff();
    }
}
