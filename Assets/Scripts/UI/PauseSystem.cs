using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PauseSystem : MonoBehaviour
{
    [SerializeField] float transitionDuration;

    [Header("References")]
    [SerializeField] PagerInteractionManager pagerInteractionManager;

    [SerializeField] GameObject shipButton;
    [SerializeField] StoryEventScriptableObject shipButtonEventTrigger;

    CanvasGroup canvasGroup;

    public static bool OnPause {get; private set;}
    public static PauseSystem Instance;

    private void Awake() 
    {
        Instance = this;   
        
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        canvasGroup.alpha = 0;

        if (pagerInteractionManager)
        {
            pagerInteractionManager.OnFocus = false;
            pagerInteractionManager.enabled = false;
        }

        if (shipButton && shipButtonEventTrigger)
        {
            shipButton.SetActive(shipButtonEventTrigger.state);
        }

        OnPause = false; 
    }
    
    public void TogglePauseState()
    {
        SetPauseState(!OnPause);
    }

    public void SetPauseState (bool value)
    {
        if (value)
            Time.timeScale = 0;
        else
            StartCoroutine( RaposUtil.Wait(1, () => Time.timeScale = 1) );
 
        OnPause = value;
        
        if (pagerInteractionManager)
        {
            pagerInteractionManager.OnFocus = value;
            pagerInteractionManager.enabled = value;
        }

        if (shipButton && shipButtonEventTrigger)
        {
            shipButton.SetActive(shipButtonEventTrigger.state);
        }
        
        canvasGroup.DOKill();
        canvasGroup.DOFade(value ? 1 : 0, transitionDuration)
            .SetUpdate(isIndependentUpdate: true);
    }

    // -- Usado na tela inicial, dá acesso somente às Options
    public void CustomSetPauseState (UnityAction backCall)
    {
        Time.timeScale = 0;
 
        OnPause = true;
        
        if (pagerInteractionManager)
        {
            pagerInteractionManager.CustomActivation(backCall);
        }

        if (shipButton)
        {
            shipButton.SetActive(false);
        }
        
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, transitionDuration)
            .SetUpdate(isIndependentUpdate: true);
    }

    public void GoToShip()
    {
        PlayerCharacter player = PlayerCharacter.Instance;
        if (player)
        {
            SetPauseState(false);

            PlayerTransitionState playerTransitionState = player.GetComponent<PlayerTransitionState>();
            playerTransitionState.TeleportOut( () => GameManager.GoToScene(BuildIndex.Ship) );

            return;
        }
        
        OnPause = false;
        GameManager.GoToScene(BuildIndex.Ship);
    }

    public void ResetScene()
    {
        GameManager.ResetScene();
    }

    public void QuitGame()
    {
        GameManager.GoToScene(BuildIndex.Title);
    }

    private void OnDisable() 
    {
        Time.timeScale = 1;
        OnPause = false;    
    }
}
