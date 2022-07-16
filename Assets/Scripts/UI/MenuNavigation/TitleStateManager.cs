using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStateManager : MonoBehaviour
{
    [SerializeField] State initialState;

    [Header("References")]
    [SerializeField] ChillMenuNavigation chillMenyNavigation;
    [SerializeField] TitleMenuNavigation titleMenuNavigation;
    [SerializeField] CanvasGroup menuCanvasGroup;

    enum State { Intro, Menu, ChillLoop }
    State state;

    void Start()
    {
        SetState(State.Intro);
    }

    private void SetState (State state)
    {
        if (chillMenyNavigation)
        {
            chillMenyNavigation.enabled = (state != State.Menu);
            chillMenyNavigation.OnFocus = (state != State.Menu);
        }

        if (titleMenuNavigation)
        {
            if (state == State.Menu) 
                titleMenuNavigation.FadeInSequence();
            else
            {
                titleMenuNavigation.enabled = false;
                titleMenuNavigation.OnFocus = false;
            }
                
        }

        if (menuCanvasGroup)
            menuCanvasGroup.alpha = (state == State.Menu ? 1 : 0);

        this.state = state;
    }

    public void SetMenuState()
    {
        if (state == State.Menu)
            return;

        SetState(State.Menu);
    }
}
