using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStateManager : MonoBehaviour
{
    [SerializeField] State initialState;

    [Header("References")]
    [SerializeField] ChillMenuNavigation chillMenuNavigation;
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

        if (chillMenuNavigation)
        {
            chillMenuNavigation.enabled = (newState == State.Intro || newState == State.ChillLoop);
            chillMenuNavigation.OnFocus = (newState == State.Intro || newState == State.ChillLoop);
        }

        if (titleMenuNavigation)
        {
            if (newState == State.Menu) 
                titleMenuNavigation.FadeInSequence();
            else
            {
                titleMenuNavigation.enabled = false;
                titleMenuNavigation.OnFocus = false;
            }
                
        }

        if (menuCanvasGroup)
            menuCanvasGroup.alpha = (newState == State.Menu ? 1 : 0);

        state = newState;
    }

    public void SetMenuState()
    {
        Debug.Log("SetMenuState");

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

}
