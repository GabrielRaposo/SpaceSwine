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

    [Header("Exclusive Buttons")]
    [SerializeField] GameObject resetRoundButton;
    [SerializeField] GameObject exitZoneButton;

    [Header(" ")]
    [SerializeField] UnityEvent backOnMainEvent;
    [SerializeField] float maxScreenDelay = .4f;

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

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event appearAKEvent;
    [SerializeField] AK.Wwise.Event vanishAKEvent;
    [SerializeField] AK.Wwise.Event callShipAKEvent;
    [SerializeField] AK.Wwise.Event keychainButtonAKEvent;
    [SerializeField] AK.Wwise.Event keychainShakeAKEvent;

    [HideInInspector] public bool OnFocus;

    int current;
    float screenDelay;
    float shipCallCount; 
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
        callScreen.SetActive(false); 

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

        if (keychainState && keychainShakeAKEvent != null)
            keychainShakeAKEvent.Post(gameObject);
    }

    private bool CheckInputBlock
    {
        get { return !OnFocus || SceneTransition.OnTransition || shipCallCount > 0; }
    }

    public void CustomActivation (UnityAction backCall)
    {
        optionsMode = true;

        if (optionsBackButton)
            optionsBackButton.OverrideInteractionEvent(backCall);

        SlideInSequence();
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
        if (exitZoneButton)
            exitZoneButton.SetActive(RoundsManager.Instance);

        if (appearAKEvent != null)
            appearAKEvent.Post(gameObject);

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

        if (vanishAKEvent != null)
            vanishAKEvent.Post(gameObject);

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

        // -- Som de chamada da Nave
        if (callShipAKEvent != null)
        {
            if (shipInput && !callShipAKEvent.IsPlaying(gameObject))
            {
                if (keychainButtonAKEvent != null)
                    keychainButtonAKEvent.Post(gameObject);

                callShipAKEvent.Post(gameObject);
            } else
            if (!shipInput && callShipAKEvent.IsPlaying(gameObject))
            {
                callShipAKEvent.FadeOut(gameObject, duration: .1f);
            }
        }
        
        // -- Contagem de tempo do timer
        if (!shipInput)
            shipCallCount = 0;
        else
        {
            if (shipCallCount > holdDuration)
            {
                if (callShipAKEvent != null)
                    callShipAKEvent.Stop(gameObject);

                callShipEvent?.Invoke();
                return;
            }
            shipCallCount += Time.unscaledDeltaTime;    
        }
        if (callCountFillDisplay)
            callCountFillDisplay.fillAmount = shipCallCount / holdDuration;

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
        Vector2 axis = Vector2.zero;
        if (navigationAction != null)
            axis = navigationAction.ReadValue<Vector2>();

        if (!OnFocus || SceneTransition.OnTransition)
            return;

        ShipInputLogic();

        if (CheckInputBlock || callingShip)
            return;

        if (confirmAction.ReadValue<float>() > .75f)
            axis = Vector2.right;
        
        pagerAxisButtonsVisual.SetAxisButtons(axis);

        if (axis == Vector2.zero)
            screenDelay = 0;

        if (screenDelay > 0)
        {
            screenDelay -= Time.unscaledDeltaTime;
            return;
        }

        if (axis.y != 0)
        {
            if (axis.y > .5f)
            {
                CurrentScreen.ChangeIndex(-1);
            }
            else if (axis.y < .5f)
            {
                CurrentScreen.ChangeIndex(+1);
            }

            screenDelay = maxScreenDelay;
            return;
        }
            
        if (axis.x != 0)
        {
            if (CurrentScreen.HorizontalInput (axis.x))
            {
                screenDelay = maxScreenDelay;
            }
            else 
            {
                if (axis.x < .75f) // -- Custom back input
                {
                    BackInput();
                    screenDelay = maxScreenDelay;
                }
                else if (axis.x > .75f) // -- Custom forward input
                {
                    CurrentScreen.ClickInput();
                    screenDelay = maxScreenDelay;
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

    
    public void SetExitZoneConfirmation()
    {
        if (!confirmationScreen)
            return;

        RoundsManager roundsManager = RoundsManager.Instance;
        if (!roundsManager)
            return;

        BuildIndex buildIndex = BuildIndex.World0Exploration; 
        if (RoundsManager.SessionData != null)
            buildIndex = RoundsManager.SessionData.outroScene;

        int previousScreen = current;

        confirmationScreen.SetScreen
        (
            title: "Exit zone?",
            description: "Progress will be lost.",
            ConfirmEvent: () => 
            {
                if (RoundsManager.SessionData)
                    SpawnManager.Index = RoundsManager.SessionData.AbandonSpawnIndex;
                PlayerTransitionState.EnterState = PlayerTransitionState.State.Teleport;
                GameManager.GoToScene(buildIndex);
            }, 
            CancelEvent: () => 
            { 
                current = previousScreen; 
                ActivateCurrent(); 
            }
        );

        GoToScreen(0);
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
        if (callShipAKEvent != null)
            callShipAKEvent.Stop(gameObject);

        navigationAction.Disable();
        confirmAction.Disable();
        playerInputActions.UI.Cancel.Disable();
        shipInputAction.Disable();

        OnFocus = false;
    }
}
