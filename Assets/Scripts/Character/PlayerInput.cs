using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Minigame;

[RequireComponent(typeof(LocalGameplayState))]
[RequireComponent(typeof(PlatformerCharacter))]
[RequireComponent(typeof(GravityInteraction))]
[RequireComponent(typeof(SpaceJumper))]
[RequireComponent(typeof(CollectableInteraction))]
[RequireComponent(typeof(PlayerInteractor))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField] InputAction testInputAction;

    PlayerInputActions playerInputActions;
    InputAction movement;
    InputAction jump;

    LocalGameplayState gameplayState;
    PlatformerCharacter platformerCharacter;
    GravityInteraction gravityInteraction;
    SpaceJumper spaceJumper;
    CollectableInteraction collectableInteraction;
    PlayerInteractor playerInteractor;

    private void Awake() 
    {
        gameplayState = GetComponent<LocalGameplayState>();
        platformerCharacter = GetComponent<PlatformerCharacter>();
        gravityInteraction = GetComponent<GravityInteraction>();
        spaceJumper = GetComponent<SpaceJumper>();
        collectableInteraction = GetComponent<CollectableInteraction>();
        playerInteractor = GetComponent<PlayerInteractor>();
    }

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();  
        
        movement = playerInputActions.Player.Movement;
        movement.Enable();

        SetJumpAction();

        playerInputActions.Player.Throw.performed += DoThrow;
        playerInputActions.Player.Throw.Enable();

        playerInputActions.Player.Interact.performed += DoInteract;
        playerInputActions.Player.Interact.Enable();

        testInputAction.performed += (ctx) => collectableInteraction.ToggleDirectionDisplay();
        testInputAction.Enable();
    }

    public void SetJumpAction()
    {
        jump = playerInputActions.Player.Jump;
        jump.performed -= DoJump;
        jump.performed -= DoLaunch;

        if (gameplayState.state == GameplayState.Exploration)
            jump.performed += DoJump;
        else
            jump.performed += DoLaunch;
        
        jump.Enable();
    }

    private void DoJump(InputAction.CallbackContext ctx)
    {
        if (IsInputBlocked())
            return;

        platformerCharacter.JumpInput();
    }

    private void DoThrow(InputAction.CallbackContext ctx)
    {
        if (IsInputBlocked())
            return;

        collectableInteraction.InteractInput();
    }

    private void DoInteract(InputAction.CallbackContext ctx)
    {
        if (IsInputBlocked())
            return;

        playerInteractor.Interact();
    }

    private void DoLaunch(InputAction.CallbackContext ctx)
    {
        if (IsInputBlocked())
            return;

        spaceJumper.JumpInput();
    }

    private bool IsInputBlocked()
    {
        if (Time.timeScale < 1)
            return true;

        if (GameManager.BlockCharacterInput || DialogueSystem.OnDialogue || SceneTransition.OnTransition)
            return true;

        return false;
    }

    void Update()
    {
        //if (DialogueSystem.OnDialogue)
        //    return;

        //if (Input.GetButtonDown("Interact"))
        //{
        //    if (playerInteractor.Interact())
        //        return;
        //}

        //platformerCharacter.HorizontalInput(Input.GetAxis("Horizontal"));
        //collectableInteraction.AxisInput(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));

        //if (Input.GetButtonDown("Jump"))
        //{
        //    platformerCharacter.JumpInput();
        //} 
        //else if (Input.GetButtonDown("Launch"))
        //{
        //    spaceJumper.JumpInput();
        //} 
        //else if (Input.GetButtonDown("Throw"))
        //{
        //    collectableInteraction.InteractInput();
        //}

        //gravityInteraction.SetJumpHeld(Input.GetButton("Jump"));           
    }

    private void FixedUpdate() 
    {
        if (DialogueSystem.OnDialogue)
            return;

        if (GameManager.BlockCharacterInput)
            return;

        Vector2 movementInput = movement.ReadValue<Vector2>();
        movementInput = movementInput.To8Normalized();

        platformerCharacter.HorizontalInput(movementInput);
        collectableInteraction.AxisInput(movementInput);

        if (gameplayState.state == GameplayState.Exploration)
            gravityInteraction.SetJumpHeld(jump.ReadValue<float>() > .5f);
    }

    private void OnDisable() 
    {
        movement.Disable();
        jump.Disable();

        playerInputActions.Player.Throw.Disable();
        playerInputActions.Player.Interact.Disable();
        playerInputActions.Player.Launch.Disable();

        testInputAction.performed -= (ctx) => collectableInteraction.ToggleDirectionDisplay();
        testInputAction.Disable();
    }
}
