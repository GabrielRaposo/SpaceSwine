using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PagerInteractionManager : MonoBehaviour
{
    [SerializeField] PagerAxisButtonsVisual pagerAxisButtonsVisual;
    [SerializeField] List<PagerScreen> screens;

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

        current = 0;

        playerInputActions = new PlayerInputActions();    

        navigationAction = playerInputActions.UI.Navigation;
        //navigationAction.performed += (ctx) => 
        //{ 
        //    if(!OnFocus || SceneTransition.OnTransition)
        //        return;

        //    Vector2 navigationInput = ctx.ReadValue<Vector2>();

        //    if (navigationInput.y != 0)
        //    {
        //        if (navigationInput.y > .5f)
        //            CurrentScreen.ChangeIndex(-1);
        //        else if (navigationInput.y < .5f)
        //            CurrentScreen.ChangeIndex(+1);
        //    }
            
        //    if (navigationInput.x != 0)
        //    {
        //        CurrentScreen.HorizontalInput (navigationInput.x);
        //    }
        //};
        navigationAction.Enable();

        playerInputActions.UI.Confirm.performed += (ctx) => 
        {               
            if(!OnFocus || SceneTransition.OnTransition)
                return;

            CurrentScreen.ClickInput();
        };
        playerInputActions.UI.Confirm.Enable();

        //OnFocus = true;
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

    private PagerScreen CurrentScreen
    {
        get {  return screens[current % screens.Count]; }
    }

    private void OnDisable() 
    {
        navigationAction.Disable();
        playerInputActions.UI.Confirm.Disable();

        OnFocus = false;
    }
}
