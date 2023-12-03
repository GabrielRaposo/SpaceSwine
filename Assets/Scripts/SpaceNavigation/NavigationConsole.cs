using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Jumper;
using Minigame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using DevLocker.Utils;

public class NavigationConsole : MonoBehaviour
{
    const int HIDDEN_Y = -1000;
    
    [SerializeField] SceneReference worldNavigationScene;

    [Header("Values")]
    [SerializeField] float duration;
    [SerializeField] float inDelay;
    [SerializeField] float outDelay;
    
    [Header("References")]
    [SerializeField] RenderTexture navigationRenderTexture;
    [SerializeField] RectTransform consoleAnchor;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event slideInAKEvent;
    [SerializeField] AK.Wwise.Event slideOutAKEvent;

    bool loadedAndActive;
    bool turnedOn;

    Sequence mainSequence;
    CanvasGroup canvasGroup;
    AsyncOperation asyncSceneLoad;
    PlayerInputActions playerInputActions;

    public UnityAction <bool> OnStateChange;

    //public static bool TurnedOn { get; private set; }
    public static NavigationConsole Instance;

    //public static string ShipTeleportScenePath = "Assets/Scenes/World1/World1Exploration-0Hub-Scene.unity";

    private void Awake() 
    {
        Instance = this;

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        // -- Temp
        //ShipTeleportSceneIndex = (int) BuildIndex.World1Exploration;
    }

    private void OnEnable() 
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        playerInputActions.UI.Cancel.performed += (ctx) =>
        {
            if (!turnedOn || NavigationShip.ControlsLocked)
                return;

            ToggleConsoleState();
        };
        playerInputActions.UI.Cancel.Enable();

        playerInputActions.UI.Start.performed += (ctx) =>
        {
            if (!turnedOn || NavigationShip.ControlsLocked)
                return;

            ToggleConsoleState();
        };
        playerInputActions.UI.Start.Enable();
    }

    public void SetTurnedOn (bool value) 
    {
        turnedOn = value;
        GameManager.BlockCharacterInput = value;
        OnStateChange?.Invoke(value);
    }


    public void ToggleConsoleState()
    {
        if (turnedOn)
        {
            if (loadedAndActive)
            {
                TurnConsoleOff();
            }
        } else TurnConsoleOn();
    }

    public void TurnConsoleOn()
    {
        SetTurnedOn(true);
        //canvasGroup.alpha = 1;

        consoleAnchor.DOKill();
        consoleAnchor.MoveY(HIDDEN_Y);

        if (mainSequence != null)
            mainSequence.Kill();

        mainSequence = DOTween.Sequence();
        //mainSequence.AppendCallback( () => {} );
        //mainSequence.AppendInterval( inDelay );
        mainSequence.Append( canvasGroup.DOFade(1, .1f) );
        mainSequence.AppendCallback( () => slideInAKEvent?.Post(gameObject) );
        mainSequence.Append
        (
            DOVirtual.Float(HIDDEN_Y, 0, duration, 
                (y) => 
                {
                    consoleAnchor.MoveY(y);    
                }
            ).SetEase(Ease.OutCirc)
        );

        mainSequence.OnComplete
        (
            () => SetupNavigationScene()
        );
    }

    private string GetNavigationScene ()
    {
        return worldNavigationScene.ScenePath;
    }

    private void SetupNavigationScene()
    {
        StartCoroutine( AsyncLoadRoutine( GetNavigationScene() ) );
    }
        
    private void UnloadNavigationScene()
    {
        if (asyncSceneLoad == null)
            return;
            
        SceneManager.UnloadSceneAsync( GetNavigationScene() );
            
        loadedAndActive = false;
    }

    public void TurnConsoleOff()
    {
        consoleAnchor.DOKill();
        consoleAnchor.MoveY(0);
        slideOutAKEvent?.Post(gameObject);

        if (mainSequence != null)
            mainSequence.Kill();

        mainSequence = DOTween.Sequence();
        mainSequence.Append
        (
            DOVirtual.Float(0, HIDDEN_Y, duration / 2f, 
                (y) => 
                {
                    consoleAnchor.MoveY(y);    
                }
            ).SetEase(Ease.InCirc)
        );
        //mainSequence.AppendInterval(outDelay);
        mainSequence.Append( canvasGroup.DOFade(0, outDelay) );
        mainSequence.OnComplete( () => SetTurnedOn(false) );

        UnloadNavigationScene();
    }

    private IEnumerator AsyncLoadRoutine(string path)
    {
        asyncSceneLoad = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
        while (!asyncSceneLoad.isDone)
            yield return new WaitForEndOfFrame();

        yield return new WaitForEndOfFrame();

        SetupComponents();
    }

    private void SetupComponents()
    {
        NavigationCameraController cameraController = NavigationCameraController.Instance;
            
        if (cameraController)
            cameraController.SetRenderTexture(navigationRenderTexture);

        NavigationSceneManager.Instance.ConectToConsole(this);

        loadedAndActive = true;
    }

    private void OnDisable() 
    {
        playerInputActions.Disable();

        playerInputActions.UI.Cancel.Disable();
        playerInputActions.UI.Start.Disable();
    }
}

