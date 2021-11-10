using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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

        bool loadedAndActive;

        CanvasGroup canvasGroup;
        AsyncOperation asyncMinigameLoad;
        GGSMinigame pluggedCard;

        public static bool TurnedOn { get; private set; }
        public static GGSConsole Instance;

        private void Awake() 
        {
            Instance = this;

            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
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
            TurnedOn = false;
            splashScreen.SetVisibility(true);
            
            consoleAnchor.DOKill();
            consoleAnchor.MoveY(0);
            DOVirtual.Float(0, HIDDEN_Y, duration / 2f, 
                (y) => 
                {
                    consoleAnchor.MoveY(y);    
                }
            ).SetEase(Ease.InCirc);

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
    }
}

public enum GGSMinigame
{
    None,
    Jumper
}