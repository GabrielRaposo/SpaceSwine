using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationSceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI autoPilotText;
    [SerializeField] private TextMeshProUGUI landedText;
    [SerializeField] private CanvasGroup landedSignObject;

    public Color uiColor;

    private Color autoPilotStartingColor;
    private NavigationConsole _navigationConsole;
    private Sequence autopilotBlinkSequence;

    public static NavigationSceneManager Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy (gameObject);
                return;    
            }
        }
        Instance = this;
        
        autoPilotStartingColor = autoPilotText.color;
    }


    public void BlinkAutoPilot()
    {
        //Debug.Log("BlinkAutoPilot()");
        autopilotBlinkSequence = DOTween.Sequence();

        autopilotBlinkSequence.Append(autoPilotText.DOColor(uiColor, 0.6f));
        autopilotBlinkSequence.Append(autoPilotText.DOColor(autoPilotStartingColor, 1f));

        autopilotBlinkSequence.SetLoops(-1);

        autopilotBlinkSequence.Play();
    }

    public void StopBlinkAutoPilot()
    {
        if(autopilotBlinkSequence == null) return;
        
        autopilotBlinkSequence.Kill();

        autoPilotText.color = autoPilotStartingColor;
    }

    public void DisplayLandingSign()
    {
        landedSignObject.DOFade(1f, 0.6f);

        var s = DOTween.Sequence();

        s.Append(landedText.DOFade(0.5f, 0.75f));
        s.Append(landedText.DOFade(0.85f, 0.75f));

        s.SetLoops(-1);
    }
    
    public void ConectToConsole(NavigationConsole nc)
    {
        _navigationConsole = nc;
    }

    public void CloseAndSetScene (string scenePath, bool callDangerTransition = false)
    {
        SetShipTeleportScene (scenePath);

        if (_navigationConsole)
            _navigationConsole.ToggleConsoleState();

        // -- TEMP
        if (_navigationConsole)
            _navigationConsole.SetTurnedOn(false);
        // --

        PlayerTransitionState.EnterState = PlayerTransitionState.State.Teleport;

        SaveManager.SetSpawnPath(scenePath);

        if (!callDangerTransition)
            GameManager.GoToScene(scenePath);
        else 
            SceneTransition.LoadScene( scenePath, SceneTransition.TransitionType.SafetyToDanger );
    }

    private void SetShipTeleportScene (string scenePath)
    {
        if (!SaveManager.Initiated)
            return;

        SaveManager.ShuttleExitLocationPath = scenePath;
        DebugDisplay.Call("ShuttleExitLocationPath set as " + scenePath);
    }
    
}
