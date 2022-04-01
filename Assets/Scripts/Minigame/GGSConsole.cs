using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Jumper;

namespace Minigame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GGSConsole : MonoBehaviour
    {
        const int HIDDEN_Y = -1000;
    
        [SerializeField] float duration;
        [SerializeField] RenderTexture minigameRenderTexture;
        [SerializeField] GGSSplashScreen splashScreen;
        [SerializeField] RectTransform consoleAnchor;

        [Header("Audio")]
        [SerializeField] AK.Wwise.Event slideInAKEvent;
        [SerializeField] AK.Wwise.Event slideOutAKEvent;

        bool loadedAndActive;

        CanvasGroup canvasGroup;
        AsyncOperation asyncMinigameLoad;
        GGSMinigame pluggedCard;
        PlayerInputActions playerInputActions;

        public static bool TurnedOn { get; private set; }
        public static GGSConsole Instance;

        private void Awake() 
        {
            Instance = this;

            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        private void OnEnable() 
        {
            playerInputActions = new PlayerInputActions();

            playerInputActions.UI.Start.performed += (ctx) =>
            {
                if (!TurnedOn)
                    return;
                ToggleConsoleState();
            };
            playerInputActions.UI.Start.Enable();
        }

        public void ToggleConsoleState()
        {
            if (TurnedOn)
            {
                if (loadedAndActive)
                {
                    TurnConsoleOff();
                }
            }
            else
            {
                TurnConsoleOn();
            }
        }

        public void TurnConsoleOn()
        {
            TurnedOn = true;
            canvasGroup.alpha = 1;

            consoleAnchor.DOKill();
            consoleAnchor.MoveY(HIDDEN_Y);
            slideInAKEvent?.Post(gameObject);
            DOVirtual.Float(HIDDEN_Y, 0, duration, 
                (y) => 
                {
                    consoleAnchor.MoveY(y);    
                }
            ).SetEase(Ease.OutCirc);

            splashScreen.Call
            (
                () => SetupMinigame (GGSMinigame.Jumper)
            );
        }

        public void TurnConsoleOff()
        {
            splashScreen.SetVisibility(true);
            
            consoleAnchor.DOKill();
            consoleAnchor.MoveY(0);
            slideOutAKEvent?.Post(gameObject);
            DOVirtual.Float(0, HIDDEN_Y, duration / 2f, 
                (y) => 
                {
                    consoleAnchor.MoveY(y);    
                }
            ).SetEase(Ease.InCirc)
            .OnComplete( () => TurnedOn = false );

            UnloadMinigame();
        }

        private void SetupMinigame (GGSMinigame ggsMinigame)
        {
            pluggedCard = ggsMinigame;

            BuildIndex buildIndex = GetCardIndex();
            StartCoroutine( AsyncLoadRoutine( (int) buildIndex) );
        }

        private BuildIndex GetCardIndex()
        {
            switch (pluggedCard)
            {
                default:
                case GGSMinigame.Jumper:
                    return BuildIndex.MinigameJumper;
            }
        }

        private IEnumerator AsyncLoadRoutine(int index)
        {
            asyncMinigameLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            while (!asyncMinigameLoad.isDone)
                yield return new WaitForEndOfFrame();

            yield return new WaitForEndOfFrame();

            SetupComponents();
        }

        private void SetupComponents()
        {
            MJ_CameraController cameraController = MJ_CameraController.Instance;
            if (cameraController)
                cameraController.SetRenderTexture(minigameRenderTexture);

            MJ_GameManager gameManager = MJ_GameManager.Instance;
            if (gameManager)
                gameManager.ConnectToConsole(this);

            loadedAndActive = true;
        }

        private void UnloadMinigame()
        {
            if (asyncMinigameLoad == null)
                return;

            BuildIndex buildIndex = GetCardIndex();
            SceneManager.UnloadSceneAsync((int) buildIndex);
            
            loadedAndActive = false;
        }

        public void ReloadMinigame()
        {
            UnloadMinigame();

            BuildIndex buildIndex = GetCardIndex();
            StartCoroutine(AsyncLoadRoutine((int) buildIndex) );
        }

        private void OnDisable() 
        {
            playerInputActions.UI.Start.Disable();
        }
    }
}

public enum GGSMinigame
{
    None,
    Jumper
}