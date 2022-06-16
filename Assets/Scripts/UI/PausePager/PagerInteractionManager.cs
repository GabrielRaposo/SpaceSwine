using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PagerInteractionManager : MonoBehaviour
{
    [SerializeField] int initialIndex;
    [SerializeField] PagerAxisButtonsVisual pagerAxisButtonsVisual;
    [SerializeField] List<PagerScreen> screens;
    [SerializeField] PagerConfirmationScreen confirmationScreen;

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

        current = 1 % screens.Count;
        ActivateCurrent();

        playerInputActions = new PlayerInputActions();    

        navigationAction = playerInputActions.UI.Navigation;
        navigationAction.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            CurrentScreen.ClickInput();
        };
        playerInputActions.UI.Confirm.Enable();
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
            title: "Quit Game",
            description: "Are you sure?",
            ConfirmEvent: () => GameManager.GoToScene(BuildIndex.Title),
            CancelEvent: () => 
            { 
                current = previousScreen; 
                ActivateCurrent(); 
            }
        );

        current = 0;
        ActivateCurrent();
    }

    private void OnDisable() 
    {
        navigationAction.Disable();
        playerInputActions.UI.Confirm.Disable();

        OnFocus = false;
    }
}
