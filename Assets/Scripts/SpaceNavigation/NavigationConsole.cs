using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Jumper;
using Minigame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class NavigationConsole : MonoBehaviour
{
    const int HIDDEN_Y = -1000;
    
        [SerializeField] float duration;
        [SerializeField] float inDelay;
        [SerializeField] float outDelay;
        [FormerlySerializedAs("minigameRenderTexture")] [SerializeField] RenderTexture navigationRenderTexture;
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

        private BuildIndex buildIndex;

        public UnityAction <bool> OnStateChange;

        //public static bool TurnedOn { get; private set; }
        public static NavigationConsole Instance;

        public static int ShipTeleportSceneIndex;

        private void Awake() 
        {
            Instance = this;

            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            // -- Temp
            ShipTeleportSceneIndex = (int) BuildIndex.World1Exploration;
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
            Debug.Log("TurnConsoleOn()");
            SetTurnedOn(true);
            canvasGroup.alpha = 1;

            consoleAnchor.DOKill();
            consoleAnchor.MoveY(HIDDEN_Y);

            if (mainSequence != null)
                mainSequence.Kill();

            mainSequence = DOTween.Sequence();
            mainSequence.AppendCallback( () => {} );
            mainSequence.AppendInterval( inDelay );
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

        private void SetupNavigationScene()
        {
            Debug.Log("SetupNavigationScene()");
            buildIndex = BuildIndex.NavigationScene;
            StartCoroutine( AsyncLoadRoutine( (int) buildIndex) );
        }
        
        private void UnloadNavigationScene()
        {
            if (asyncSceneLoad == null)
                return;
            
            SceneManager.UnloadSceneAsync((int) buildIndex);
            
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
            mainSequence.AppendInterval(outDelay);
            mainSequence.OnComplete( () => SetTurnedOn(false) );

            UnloadNavigationScene();
        }

        private IEnumerator AsyncLoadRoutine(int index)
        {
            Debug.Log("AsyncLoadRoutine(int index)");
            asyncSceneLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
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
            
            //MJ_GameManager gameManager = MJ_GameManager.Instance;
            
            //if (gameManager)
                //gameManager.ConnectToConsole(this);

            loadedAndActive = true;
        }

        private void OnDisable() 
        {
            playerInputActions.UI.Start.Disable();
        }
}

