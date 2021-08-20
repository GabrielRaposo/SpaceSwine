using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PauseSystem : MonoBehaviour
{
    [SerializeField] float transitionDuration;

    [Header("References")]
    [SerializeField] MenuNavigationManager menuNavigationManager; 

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

        if (menuNavigationManager)
            menuNavigationManager.OnFocus = false;

        OnPause = false; 
    }
    
    public void TogglePauseState()
    {
        SetPauseState(!OnPause);
    }

    public void SetPauseState(bool value)
    {
        if (value)
            Time.timeScale = 0;
        else
            StartCoroutine( RaposUtil.Wait(1, () => Time.timeScale = 1) );
 
        OnPause = value;
        
        if (menuNavigationManager)
            menuNavigationManager.OnFocus = value;
        
        canvasGroup.DOKill();
        canvasGroup.DOFade(value ? 1 : 0, transitionDuration)
            .SetUpdate(isIndependentUpdate: true);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDisable() 
    {
        Time.timeScale = 1;
        OnPause = false;    
    }
}
