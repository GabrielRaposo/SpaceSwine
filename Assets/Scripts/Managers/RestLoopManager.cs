using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RestLoopManager : MonoBehaviour
{
    [SerializeField] GameObject restLoopAnimation;
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] PlaylistPlayer playlistPlayer;
    [SerializeField] CustomTimeCounter customTimeCounter;
    
    [Header("Custom Camera")]
    [SerializeField] float zoomSize;
    [SerializeField] CanvasGroup horizontalLetterboxes;

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

        if (horizontalLetterboxes)
            horizontalLetterboxes.alpha = 0;

        customTimeCounter.OnTimeReached += (index) =>
        {
            Debug.Log("Set achievement");
            //AchievementsManager.GetAchievementState()
        };
    }

    public void TurnOn() 
    {
        GameManager.BlockCharacterInput = true;

        FadeCanvas.Call
        (
            midFadeAction: () => 
            {
                restLoopAnimation.SetActive(true);
                playerCharacter.SetHiddenState(true);
                SetCameraState(true);
            },
            afterFadeAction: () => 
            {
                if (playlistPlayer)
                {
                    playlistPlayer.SetPlayerState(true);
                    this.Wait(1, () => { playlistPlayer.OnFocus = true; });

                    customTimeCounter.Restart();
                }
            }
        );

        //TurnControlsOn();

    }

    public void TurnOff()
    {
        if (playlistPlayer) 
        {
            playlistPlayer.OnFocus = false;
        }

        FadeCanvas.Call
        (
            midFadeAction: () => 
            {
                restLoopAnimation.SetActive(false);
                playerCharacter.SetHiddenState(false);
                SetCameraState(false);

                customTimeCounter.Stop();
            },
            afterFadeAction: () => 
            {
                RaposUtil.Wait(this, frames: 2, () => 
                {
                    GameManager.BlockCharacterInput = false;
                });
            }
        );
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

    private void SetCameraState (bool value)
    {
        Camera camera = Camera.main;
        CustomShipCamera customShipCamera = camera.GetComponent<CustomShipCamera>();
        if (customShipCamera)
        {
            // TO-DO: transicionar aos poucos pro estados
            if (value)
                customShipCamera.SetCustomZoom(zoomSize);
            else
                customShipCamera.ResetZoom();

            //customShipCamera.SetCustomRotation(value ? 2f : 0f);
        }   

        if (horizontalLetterboxes)
            horizontalLetterboxes.alpha = value ? 1 : 0;
    }

    private void OnDisable() 
    {
        playerCharacter.SetHiddenState(false);
        TurnControlsOff();
        customTimeCounter.Stop();
    }
}
