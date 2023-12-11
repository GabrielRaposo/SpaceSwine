using DG.Tweening;
using Minigame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GGSMenuManager : StoryEventDependent
{
    [SerializeField] Transform verticalLayout;
    [SerializeField] List<StoryEventScriptableObject> minigameUnlockEvents;

    [Header("Exclamation Icon")]
    [SerializeField] ShipExclamationIcon shipExclamationIcon;
    [SerializeField] List<string> notificationIDs; 

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event navigationAKEvent;
    [SerializeField] AK.Wwise.Event cancelAKEvent;

    bool OnFocus;
    int index = -1;
    List<GGSMenuTab> menuTabs;

    CanvasGroup canvasGroup;
    PlayerInputActions inputActions;
    PlayerInteractor interactor;

    public static GGSMenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    void Start()
    {
        if (!verticalLayout)
            return;
        
        menuTabs = new List<GGSMenuTab>();
        verticalLayout.GetComponentsInChildren(menuTabs);

        CallDependentAction 
        ( 
            action: () => 
            {
                UpdateMinigameScores();
                UpdateTabsInteractable(true);

                SetupNotifications();
                SetExclamationIcon();
            }, 
            extraFrames: 1 
        );

        index = 0;
        UpdateTabsHighlight();
    }

    private void SetupNotifications()
    {
        if (notificationIDs == null || notificationIDs.Count < 1)
            return;

        for (int i = 0; i < notificationIDs.Count; i++)
        {
            if ( UINotificationManager.Check(notificationIDs[i]) )
                continue;

            if ( !StoryEventsManager.IsComplete (minigameUnlockEvents[i % minigameUnlockEvents.Count]) )
                continue;

            UINotificationManager.Create (notificationIDs[i]);
        }
    }

    private void SetExclamationIcon()
    {
        if (shipExclamationIcon == null)
            return;

        if (menuTabs != null && menuTabs.Count > 0 ) 
        {
            for (int i = 0; i < notificationIDs.Count && i < menuTabs.Count; i++)
            {
                menuTabs[i].SetExclamationIcon(UINotificationManager.Check(notificationIDs[i]));                
            }
        }
 
        for (int i = 0; i < notificationIDs.Count; i++)
        {
            if ( UINotificationManager.Check(notificationIDs[i]) )
            {
                shipExclamationIcon.Show();
                return;
            }
        }
    }

    private void UseNotification()
    {
        if (notificationIDs == null || notificationIDs.Count < 1)
            return;

        string notification = notificationIDs[index % notificationIDs.Count];
        if ( !UINotificationManager.Check(notification) )
                return;

        UINotificationManager.Use (notification);
    }

    public void ActivateInputs (PlayerInteractor interactor) 
    {
        this.interactor = interactor;    

        if (OnFocus)
            return;    

        inputActions = new PlayerInputActions();

        inputActions.UI.Navigation.performed += NavigationInput;
        inputActions.UI.Navigation.Enable();

        inputActions.UI.Confirm.performed += ConfirmInput;
        inputActions.UI.Confirm.Enable();

        inputActions.UI.Other.performed += ConfirmInput;
        inputActions.UI.Other.Enable();

        inputActions.UI.Cancel.performed += CancelInput;
        inputActions.UI.Cancel.Enable();

        inputActions.Enable();

        OnFocus = true;
        UpdateMinigameScores();
        canvasGroup.alpha = 1f;
        if (shipExclamationIcon)
            shipExclamationIcon.Hide();

        SetInteractorState (lockMovement: true);
    }

    private void SetInteractorState (bool lockMovement)
    {
        if (interactor == null)
            return;

        if (lockMovement)
        {
            PlayerInput playerInput = interactor.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = false;

            PlatformerCharacter platformer = interactor.GetComponent<PlatformerCharacter>();
            if (platformer) platformer.KillInputs();
        }
        else
        {
            PlayerInput playerInput = interactor.GetComponent<PlayerInput>();
            if (playerInput) playerInput.enabled = true;
        }
    }

    private void UpdateMinigameScores()
    {
        for (int i = 0; i < menuTabs.Count; i++) 
        {
            int score = -1;

            switch (i) 
            {
                case 0: score = SaveManager.GetHighscore (GGSConsole.Jumper);   break;
                case 1: score = SaveManager.GetHighscore (GGSConsole.Shooter);  break;
                case 2: score = SaveManager.GetHighscore (GGSConsole.Traveler); break;
            }
            
            menuTabs[i].SetScore(score);
        }
    }

    private void UpdateTabsInteractable(bool aux)
    {
        if (!aux)
            return;

        for (int i = 0; i < menuTabs.Count - 1; i++)
        {
            bool state = i < minigameUnlockEvents.Count && StoryEventsManager.IsComplete(minigameUnlockEvents[i]);
            menuTabs[i].SetInteractableState(state);
        }
        menuTabs[menuTabs.Count - 1].SetInteractableState(true);
    }

    private void UpdateTabsHighlight()
    {
        if (menuTabs.Count < 1)
            return;

        for (int i = 0; i < menuTabs.Count; i++) 
            menuTabs[i].SetHighlightState (i == index);
    }

    private void NavigationInput (InputAction.CallbackContext ctx)
    {
        if (navigationAKEvent != null)
            navigationAKEvent.Post(gameObject);

        if (!OnFocus || PauseSystem.OnPause)
            return;

        Vector2 axis = ctx.ReadValue<Vector2>();
        if (axis.y == 0)
            return;

        MoveCursor (axis.y > 0 ? -1 : 1);
    }

    private void MoveCursor (int value)
    {
        index += value;
        if (index < 0)
            index = menuTabs.Count - 1;
        index %= menuTabs.Count;

        UpdateTabsHighlight();
    }

    private void ConfirmInput (InputAction.CallbackContext ctx)
    {
        if (!OnFocus || PauseSystem.OnPause)
            return;

        menuTabs[index % menuTabs.Count].OnSubmit();
    }

    public void OnSubmit() // -- Chamada externa
    {
        GGSConsole ggsConsole = GGSConsole.Instance; 
        if (!ggsConsole)
            return;

        UseNotification();
        ggsConsole.ToggleConsoleState();

        ExitMenu();
    }

    private void CancelInput (InputAction.CallbackContext ctx)
    {
        if (!OnFocus || PauseSystem.OnPause)
            return;

        ExitMenu();
    }

    public void ExitMenu()
    {
        if (cancelAKEvent != null)
            cancelAKEvent.Post(gameObject);

        if (!OnFocus)
            return;

        inputActions.UI.Navigation.Disable();

        inputActions.UI.Confirm.Disable();
        inputActions.UI.Other.Disable();
        inputActions.UI.Cancel.Disable();

        inputActions.Disable();

        canvasGroup.alpha = 0;
        OnFocus = false;

        SetExclamationIcon();

        SetInteractorState (lockMovement: false);
    }

    private void OnDisable()
    {
        if (!OnFocus)
            return;

        inputActions.UI.Navigation.Disable();

        inputActions.UI.Confirm.Disable();
        inputActions.UI.Other.Disable();
        inputActions.UI.Cancel.Disable();

        inputActions.Disable();

        OnFocus = false;
    }
}