using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PagerInteractionManager : MonoBehaviour
{
    [SerializeField] int initialIndex;
    [SerializeField] PagerAxisButtonsVisual pagerAxisButtonsVisual;
    [SerializeField] PagerConfirmationScreen confirmationScreen;
    [SerializeField] UnityEvent backOnMainEvent;
    [SerializeField] List<PagerScreen> screens;

    [Header("Options Mode")]
    [SerializeField] bool optionsMode;
    [SerializeField] PagerInteractableButton optionsBackButton;

    [HideInInspector] public bool OnFocus;
    
    int current;
    float delay;

    PlayerInputActions playerInputActions;
    InputAction navigationAction;

    private void OnEnable() 
    {
        if (screens.Count < 1 || pagerAxisButtonsVisual == null)
        {
            gameObject.SetActive(false);
            return;
        }

        int i = initialIndex > -1 ? initialIndex : 1;
        if (optionsMode) i = 2;

        GoToScreen ( i );

        //if (optionsMode && optionsBackButton)
        //    optionsBackButton.gameObject.SetActive(false);

        playerInputActions = new PlayerInputActions();

        navigationAction = playerInputActions.UI.Navigation;
        navigationAction.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if (!OnFocus || SceneTransition.OnTransition)
                return;

            CurrentScreen.ClickInput();
        };
        playerInputActions.UI.Confirm.Enable();

        playerInputActions.UI.Cancel.performed += (ctx) => 
        {
            if (!OnFocus || SceneTransition.OnTransition)
                return;

            BackInput();
        };
        playerInputActions.UI.Cancel.Enable();
    }

    public void CustomActivation (UnityAction backCall)
    {
        optionsMode = true;
        OnFocus = true;
        enabled = true;

        if (optionsBackButton)
            optionsBackButton.OverrideInteractionEvent(backCall);
    }

    private void Update() 
    {
        if(!OnFocus || SceneTransition.OnTransition)
            return;

        Vector2 navigationInput = navigationAction.ReadValue<Vector2>();
        pagerAxisButtonsVisual.ReadOnline(navigationInput);

        if (navigationInput == Vector2.zero)
            delay = 0;

        if (delay > 0)
        {
            delay -= Time.deltaTime;
            return;
        }

        float maxDelay = 1.0f;

        if (navigationInput.y != 0)
        {
            if (navigationInput.y > .5f)
                CurrentScreen.ChangeIndex(-1);
            else if (navigationInput.y < .5f)
                CurrentScreen.ChangeIndex(+1);

            delay = maxDelay;
            return;
        }
            
        if (navigationInput.x != 0)
        {
            CurrentScreen.HorizontalInput (navigationInput.x);
            delay = maxDelay;
        }
    }

    private void ActivateCurrent()
    {
        foreach(PagerScreen screen in screens)
            screen.gameObject.SetActive(false);

        CurrentScreen.gameObject.SetActive(true);
    }

    private PagerScreen CurrentScreen
    {
        get {  return screens[current % screens.Count]; }
    }

    public void SetQuitConfirmation()
    {
        if (!confirmationScreen)
            return;

        int previousScreen = current;

        confirmationScreen.SetScreen
        (
            title: "Sair do jogo",
            description: "Tem certeza?",
            ConfirmEvent: () => GameManager.GoToScene(BuildIndex.Title),
            CancelEvent: () => 
            { 
                current = previousScreen; 
                ActivateCurrent(); 
            }
        );

        GoToScreen(0);
    }

    public void GoToScreen (int index)
    {
        current = index;
        ActivateCurrent();
    }

    public void BackInput()
    {
        switch (current)
        {
            case 1: // -- Tela principal
                backOnMainEvent?.Invoke();
                break;

            case 0: // -- Menu de Confirmação
                GoToScreen (1);
                break;

            case 2: // -- Menu de Opções
                if (optionsMode) break;
                GoToScreen (1);
                break;

            case 3: // -- Menu de Opções (Áudio)
                GoToScreen (2);
                break;
        }
    }

    private void OnDisable() 
    {
        navigationAction.Disable();
        playerInputActions.UI.Confirm.Disable();
        playerInputActions.UI.Cancel.Disable();

        OnFocus = false;
    }
}
