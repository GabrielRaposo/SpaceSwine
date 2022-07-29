using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStateManager : MonoBehaviour
{
    [SerializeField] State initialState;

    [Header("References")]
    [SerializeField] PlaylistPlayer playlistPlayer;
    [SerializeField] TitleMenuNavigation titleMenuNavigation;
    [SerializeField] PauseSystem pauseSystem;
    [SerializeField] CanvasGroup menuCanvasGroup;

    enum State { Intro, Menu, Options, ChillLoop }
    State state;

    void Start()
    {
        SetState(State.Intro);
    }

    private void SetState (State newState)
    {
        if (pauseSystem)
        {
            if (state != State.Options && newState == State.Options)
            {
                pauseSystem.CustomSetPauseState( backCall: () => SetMenuState() );    
            } 
            
            if (state == State.Options && newState != State.Options)
            {
                pauseSystem.SetPauseState(false);
            }
        }

        if (playlistPlayer)
        {
            playlistPlayer.enabled = (newState == State.Intro || newState == State.ChillLoop);
            playlistPlayer.OnFocus = (newState == State.Intro || newState == State.ChillLoop);

            playlistPlayer.SetPlayerMode( newState == State.ChillLoop );
            if (newState == State.ChillLoop)
                playlistPlayer.SetPlayerState(true);

        }

        if (titleMenuNavigation && menuCanvasGroup)
        {
            if (newState == State.Menu)
            {
                if (state != State.Options)
                {
                    titleMenuNavigation.FadeInSequence();
                }
                else
                {
                    titleMenuNavigation.enabled = true;
                    titleMenuNavigation.OnFocus = true;
                }
                menuCanvasGroup.alpha = 1;
            }
            else
            {
                if (newState == State.Options)
                {
                    titleMenuNavigation.enabled = false;
                    titleMenuNavigation.OnFocus = false;
                }
                else 
                {
                    if (state == State.Menu)
                    {
                        titleMenuNavigation.FadeOutSequence();
                    }
                    else 
                    {
                        titleMenuNavigation.enabled = false;
                        titleMenuNavigation.OnFocus = false;
                        menuCanvasGroup.alpha = 0;
                    }
                }
            }
                
        }

        state = newState;
    }

    public void SetMenuState()
    {
        //Debug.Log("SetMenuState");

        if (state == State.Menu)
            return;

        SetState(State.Menu);
    }

    public void SetOptionsState() 
    {
        if (state == State.Options)
            return;

        SetState(State.Options);
    }

    public void SetChillState() 
    {
        if (state == State.ChillLoop)
            return;

        SetState(State.ChillLoop);
    }

}
