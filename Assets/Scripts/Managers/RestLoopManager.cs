using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RestLoopManager : MonoBehaviour
{
    [SerializeField] GameObject restLoopAnimation;
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] PlaylistPlayer playlistPlayer;

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

        if (playlistPlayer)
        {
            playlistPlayer.SetPlayerMode(true);
            playlistPlayer.SetPlayerState(false);
        }
    }

    public void TurnOn()
    {
        GameManager.BlockCharacterInput = true;
        restLoopAnimation.SetActive(true);
        playerCharacter.SetHiddenState(true);
        //TurnControlsOn();

        if (playlistPlayer)
        {
            playlistPlayer.SetPlayerState(true);
            playlistPlayer.OnFocus = true;
        }
    }

    public void TurnOff()
    {
        if (playlistPlayer) 
        {
            playlistPlayer.OnFocus = false;
        }
        restLoopAnimation.SetActive(false);
        playerCharacter.SetHiddenState(false);

        RaposUtil.Wait(this, frames: 2, () => 
        {
            GameManager.BlockCharacterInput = false;
        });
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
        return;

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
