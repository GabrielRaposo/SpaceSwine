using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChillMenuNavigation : MonoBehaviour
{
    [HideInInspector] public bool OnFocus;

    PlayerInputActions playerInputActions;
    TitleStateManager titleStateManager;
    PlaylistPlayer playlistPlayer;

    private void Awake() 
    {
        titleStateManager = GetComponentInParent<TitleStateManager>();    
        playlistPlayer = GetComponentInChildren<PlaylistPlayer>();
    }

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();    
    
        playerInputActions.UI.Navigation.performed += (ctx) => 
        { 
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            Vector2 navigationInput = ctx.ReadValue<Vector2>();

            if (navigationInput.x != 0)
            {
                if (navigationInput.x > .5f)
                    MoveCursor(-1);
                else if (navigationInput.x < .5f)
                    MoveCursor(1);
            }
        };
        playerInputActions.UI.Navigation.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            if (titleStateManager)
                titleStateManager.SetMenuState();

            SetPlayerState(false);
        };
        playerInputActions.UI.Confirm.Enable();
    }

    public void SetPlayerState(bool value)
    {
        Debug.Log("value: " + value);
        if (value)
            playlistPlayer.SlideIn();
        else 
            playlistPlayer.SlideOut();
    }

    private void MoveCursor (int direction)
    {
        if (!playlistPlayer)
            return;

        playlistPlayer.SkipMusic(direction);
    }

    private void OnDisable() 
    {
        playerInputActions.UI.Navigation.Disable();
        playerInputActions.UI.Confirm.Disable();
    }

}
