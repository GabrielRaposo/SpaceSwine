using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using DevLocker.Utils;
using Jumper;

namespace Minigame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GGSConsole : MonoBehaviour
    {
        const int HIDDEN_Y = -1000;
    
        [SerializeField] SceneReference jumperScene;

        [Header("Values")]
        [SerializeField] float duration;
        [SerializeField] float inDelay;
        [SerializeField] float outDelay;
        
        [Header("References")]
        [SerializeField] RenderTexture minigameRenderTexture;
        [SerializeField] GGSSplashScreen splashScreen;
        [SerializeField] RectTransform consoleAnchor;

        [Header("Audio")]
        [SerializeField] AK.Wwise.Event slideInAKEvent;
        [SerializeField] AK.Wwise.Event slideOutAKEvent;

        bool loadedAndActive;
        bool turnedOn;

        Sequence mainSequence;
        CanvasGroup canvasGroup;
        AsyncOperation asyncMinigameLoad;
        GGSMinigame pluggedCard;
        PlayerInputActions playerInputActions;

        public UnityAction <bool> OnStateChange;

        public static string Jumper => "Jumper";
        public static string Shooter => "Shooter";
        public static string Traveler => "Traveler";

        //public static bool TurnedOn { get; private set; }
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
                if (!turnedOn)
                    return; 
                ToggleConsoleState();
            };
            playerInputActions.UI.Start.Enable();
        }

        private void SetTurnedOn (bool value) 
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
            }
            else
            {
                TurnConsoleOn();
            }
        }

        public void TurnConsoleOn()
        {
            //SetTurnedOn(true);
            GameManager.BlockCharacterInput = true;

            FadeCanvas.Call
            (
                midFadeAction: () => 
                {
                    turnedOn = true;
                    OnStateChange?.Invoke(true);
                },
                afterFadeAction: () =>
                {
                    consoleAnchor.DOKill();
                    consoleAnchor.MoveY(HIDDEN_Y);

                    if (mainSequence != null)
                        mainSequence.Kill();

                    mainSequence = DOTween.Sequence();
                    mainSequence.AppendCallback( () => {} );
                    mainSequence.AppendInterval( inDelay/2f );
                    mainSequence.Append( canvasGroup.DOFade(1, inDelay/2f) );
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
                        () => splashScreen.Call
                        (
                            () => SetupMinigame (GGSMinigame.Jumper)
                        )
                    );
                }
            );
        }

        public void TurnConsoleOff()
        {
            splashScreen.SetVisibility(true);
            UnloadMinigame();
            
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
            mainSequence.OnComplete( () => 
            {
                FadeCanvas.Call
                (
                    midFadeAction: () =>
                    {
                        SetTurnedOn(false); 
                    },
                    afterFadeAction: () =>
                    {
                    }
                );
            });

        }

        private void SetupMinigame (GGSMinigame ggsMinigame)
        {
            pluggedCard = ggsMinigame;

            MinigameManager.ConnectToConsole(this); //-- tornar genérico

            string cardPath = GetCardPath();
            StartCoroutine( AsyncLoadRoutine( cardPath ) );
        }

        private string GetCardPath()
        {
            switch (pluggedCard)
            {
                default:
                case GGSMinigame.Jumper:
                    return jumperScene.ScenePath;
            }
        }

        private IEnumerator AsyncLoadRoutine(string path)
        {
            asyncMinigameLoad = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
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


            loadedAndActive = true;
        }

        private void UnloadMinigame()
        {
            if (asyncMinigameLoad == null)
                return;

            string cardPath = GetCardPath();
            SceneManager.UnloadSceneAsync( cardPath );
            
            loadedAndActive = false;
        }

        public void ReloadMinigame()
        {
            UnloadMinigame();

            string cardPath = GetCardPath();
            StartCoroutine(AsyncLoadRoutine( cardPath ) );
        }

        public int GetHighscore (string minigame)
        {
            return SaveManager.GetHighscore(minigame);
        }

        public void SetHighscore (string minigame, int score)
        {
            SaveManager.SetHighscore(minigame, score);
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
    Jumper,
    Shooter,
    Traveler
}