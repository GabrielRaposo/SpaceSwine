﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;
using DevLocker.Utils;

[RequireComponent(typeof(CanvasGroup))]
public class PauseSystem : MonoBehaviour
{
    [SerializeField] float transitionDuration;

    [Header("References")]
    [SerializeField] SceneReference titleScene;
    [SerializeField] SceneReference shipScene;
    [SerializeField] PagerInteractionManager pagerInteractionManager;

    //[SerializeField] StoryEventScriptableObject shipButtonEventTrigger;

    CanvasGroup canvasGroup;

    public static UnityAction OnPauseAction;

    public static bool OnPause {get; private set;}
    public static PauseSystem Instance;

    private void Awake() 
    {
        Instance = this;   
        
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        canvasGroup.alpha = 0;

        if (pagerInteractionManager)
        {
            // -- TO-DO: Instant move away and disable

            pagerInteractionManager.OnFocus = false;
            pagerInteractionManager.enabled = false;
        }

        //if (shipButton && shipButtonEventTrigger)
        //{
        //    shipButton.SetActive( StoryEventsManager.IsComplete (shipButtonEventTrigger) );
        //}

        OnPause = false; 
    }
    
    public void TogglePauseState()
    {
        SetPauseState(!OnPause);
    }

    public void SetPauseState (bool value)
    {
        if (value)
            Time.timeScale = 0;
        else
            StartCoroutine( RaposUtil.Wait(1, () => Time.timeScale = 1) );
 
        OnPause = value;

        if (value && OnPauseAction != null)
            OnPauseAction.Invoke();

        if (pagerInteractionManager)
        {
            if (value)
            {
                pagerInteractionManager.SlideInSequence();
            }
            else
            {
                pagerInteractionManager.SlideOutSequence();
            }
        }

        //if (shipButton && shipButtonEventTrigger)
        //{
        //    shipButton.SetActive( StoryEventsManager.IsComplete (shipButtonEventTrigger) );
        //}
        
        canvasGroup.DOKill();
        canvasGroup.DOFade(value ? 1 : 0, transitionDuration)
            .SetUpdate(isIndependentUpdate: true);
    }

    // -- Usado na tela inicial, dá acesso somente às Options
    public void CustomSetPauseState (UnityAction backCall)
    {
        Time.timeScale = 0;
 
        OnPause = true;
        
        if (pagerInteractionManager)
        {
            pagerInteractionManager.CustomActivation(backCall);
        }

        //if (shipButton)
        //{
        //    shipButton.SetActive(false);
        //}

        canvasGroup.DOKill();
        canvasGroup.DOFade(1, transitionDuration)
            .SetUpdate(isIndependentUpdate: true);
    }

    public void GoToShip()
    {
        PlayerCharacter player = PlayerCharacter.Instance;
        if (player)
        {
            SetPauseState(false);

            PlayerTransitionState playerTransitionState = player.GetComponent<PlayerTransitionState>();
            playerTransitionState.TeleportOut( () => GameManager.GoToScene(shipScene.ScenePath) );

            return;
        }
        
        OnPause = false;
        GameManager.GoToScene(shipScene.ScenePath);
    }

    public void CallResetRound()
    {
        RoundsManager roundsManager = RoundsManager.Instance;
        if (!roundsManager)
            return;

        SetPauseState(false);
        roundsManager.CallReset();
    }

    public void CallExitZone()
    {
        RoundsManager roundsManager = RoundsManager.Instance;
        if (!roundsManager)
            return;

        SetPauseState(false);

    }

    public void ResetScene()
    {
        GameManager.ResetScene();
    }

    public void QuitGame()
    {
        if (titleScene == null)
            return;

        GameManager.GoToScene(titleScene.ScenePath);
    }

    private void OnDisable() 
    {
        Time.timeScale = 1;
        OnPause = false;    
    }
}
