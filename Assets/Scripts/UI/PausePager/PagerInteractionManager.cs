using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PagerInteractionManager : MonoBehaviour
{
    [SerializeField] int initialIndex;
    [SerializeField] PagerAxisButtonsVisual pagerAxisButtonsVisual;
    [SerializeField] PagerConfirmationScreen confirmationScreen;
    [SerializeField] List<PagerScreen> screens;
    [SerializeField] GameObject callScreen;
    [SerializeField] Image callCountFillDisplay;
    [SerializeField] GameObject resetRoundButton;
    [SerializeField] UnityEvent backOnMainEvent;

    [Header("Keychain Interaction")]
    [SerializeField] GameObject keychainObject;
    [SerializeField] float holdDuration;
    [SerializeField] ImageSwapper keychainSwapper;
    [SerializeField] StoryEventScriptableObject unlockStoryEvent; 
    [SerializeField] KeychainInputBanner keychainBanner;
    [SerializeField] UnityEvent callShipEvent;

    [Header("Transition Data")]
    [SerializeField] float offscreenX;
    [SerializeField] float slideDuration;

    [Header("Options Mode")]
    [SerializeField] bool optionsMode;
    [SerializeField] PagerInteractableButton optionsBackButton;

    [HideInInspector] public bool OnFocus;

    int current;
    float delay;
    float holdCount; 
    bool keychainState;
    bool callingShip;
    Sequence s;

    RectTransform rt;
    Animator animator;

    PlayerInputActions playerInputActions;
    InputAction navigationAction;
    InputAction confirmAction;
    InputAction shipInputAction;

    private void Awake() 
    {
        rt = GetComponent<RectTransform>();
        animator = GetComponent<Animator>();
    }

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

        // -- Setup de inputs
        {
            playerInputActions = new PlayerInputActions();

            navigationAction = playerInputActions.UI.Navigation;
            navigationAction.Enable();

            confirmAction = playerInputActions.UI.Confirm;
            confirmAction.Enable();

            playerInputActions.UI.Cancel.performed += (ctx) => 
            {
                if (CheckInputBlock || callingShip)
                    return;

                BackInput();
            };
            playerInputActions.UI.Cancel.Enable();

            shipInputAction = playerInputActions.UI.Other;
            shipInputAction.Enable();
        }
    }

    private void KeychainInitiationLogic()
    {
        if (unlockStoryEvent != null)
            keychainState = unlockStoryEvent.state;

        if (GameManager.IsOnScene(BuildIndex.Ship) || GameManager.IsOnScene(BuildIndex.Title))
            keychainState = false;

        if (keychainObject)
            keychainObject.SetActive( keychainState );

        if (keychainBanner)
            keychainBanner.Show();
    }

    private bool CheckInputBlock
    {
        get { return !OnFocus || SceneTransition.OnTransition || holdCount > 0; }
    }

    public void CustomActivation (UnityAction backCall)
    {
        KeychainInitiationLogic();

        if (s != null)
            s.Kill();
        SetAbsolutePosition(shown: true);
        animator.SetTrigger("Reset");

        optionsMode = true;

        OnFocus = true;
        enabled = true;

        if (optionsBackButton)
            optionsBackButton.OverrideInteractionEvent(backCall);
    }
    public void SetAbsolutePosition (bool shown)
    {
        rt = GetComponent<RectTransform>();
        rt.MoveX(shown ? 0 : offscreenX);
    }

    public void SlideInSequence() 
    {
        if (s != null)
            s.Kill();

        rt = GetComponent<RectTransform>();
        
        SetAbsolutePosition(false);

        int i = initialIndex > -1 ? initialIndex : 1;
        if (optionsMode) i = 2;
        
        GoToScreen ( i );
        callScreen.SetActive(false); 
        KeychainInitiationLogic();

        if (resetRoundButton)
            resetRoundButton.SetActive (RoundsManager.Instance);
        

        animator.SetTrigger("Reset");
        animator.SetInteger("Slide", 1);

        s = DOTween.Sequence();
        s.Append
        ( 
            DOVirtual.Float(offscreenX, 0, slideDuration, f => rt.MoveX(f) )
                .SetEase(Ease.OutCirc)
        );
        s.SetUpdate(isIndependentUpdate: true);

        s.OnComplete
        (
            () => {
                SetAbsolutePosition(true);
                animator.SetInteger("Slide", 0);
                
                enabled = true;
                OnFocus = true;
            }
        );
    }

    public void SlideOutSequence() 
    {
        if (s != null)
            s.Kill();

        rt = GetComponent<RectTransform>();
        SetAbsolutePosition(true);

        if (keychainBanner)
            keychainBanner.Hide();

        animator.SetTrigger("Reset");
        animator.SetInteger("Slide", -1);

        enabled = false;
        OnFocus = false;

        s = DOTween.Sequence();
        s.Append
        ( 
            DOVirtual.Float(0, offscreenX, slideDuration, f => rt.MoveX(f) )
                .SetEase(Ease.InCirc)
        );
        s.SetUpdate(isIndependentUpdate: true);

        s.OnComplete
        (
            () => {
                SetAbsolutePosition(false);
                animator.SetInteger("Slide", 0);
            }
        );
    }

    private void ShipInputLogic()
    {
        if (!keychainState)
            return;

        bool shipInput = shipInputAction.ReadValue<float>() > .5f;
        keychainSwapper.SetSpriteState(shipInput ? 1 : 0);

        // -- Contagem de tempo do timer
        if (!shipInput)
            holdCount = 0;
        else
        {
            if (holdCount > holdDuration)
            {
                callShipEvent?.Invoke();
                return;
            }
            holdCount += Time.fixedUnscaledDeltaTime;    
        }
        if (callCountFillDisplay)
            callCountFillDisplay.fillAmount = holdCount / holdDuration;

        // -- Mostra a tela certa
        callScreen.SetActive(shipInput);
        if (callingShip)
        {
            if (!shipInput)
                CurrentScreen.gameObject.SetActive(true);    
        }
        else
        {
            if (shipInput)
                CurrentScreen.gameObject.SetActive(false);    
        }
        callingShip = shipInput;
    }

    private void Update() 
    {
        if (!OnFocus || SceneTransition.OnTransition)
            return;

        ShipInputLogic();

        if (CheckInputBlock || callingShip)
            return;

        Vector2 navigationInput = navigationAction.ReadValue<Vector2>();
        if (confirmAction.ReadValue<float>() > .5f)
            navigationInput = Vector2.right;
        
        pagerAxisButtonsVisual.SetAxisButtons(navigationInput);

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
            if (CurrentScreen.HorizontalInput (navigationInput.x))
            {
                delay = maxDelay;
            }
            else 
            {
                if (navigationInput.x < .75f) // -- Custom back input
                {
                    BackInput();
                    delay = maxDelay;
                }
                else if (navigationInput.x > .75f) // -- Custom forward input
                {
                    CurrentScreen.ClickInput();
                    delay = maxDelay;
                }
            }
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
            title: "Quit game",
            description: "Are you sure?",
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
        confirmAction.Disable();
        playerInputActions.UI.Cancel.Disable();
        shipInputAction.Disable();

        OnFocus = false;
    }
}
