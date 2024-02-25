using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA;

public class NavigationSceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI autoPilotText;
    [SerializeField] private CanvasGroup landingSignObject;
    [SerializeField] private TextMeshProUGUI planetLandingText;
    [SerializeField] private TextMeshProUGUI portalLandingText;

    public Color uiColor;

    private Color autoPilotStartingColor;
    private NavigationConsole _navigationConsole;
    private Sequence autopilotBlinkSequence;
    private NavigationWorldManager worldManager;

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
        
        worldManager = GetComponent<NavigationWorldManager>();
        autoPilotStartingColor = autoPilotText.color;
    }

    private Color UIColor
    {
        get 
        {
            if (worldManager == null)
                return uiColor;

            NavigationWorldGroup worldGroup = worldManager.GetWorldGroup();
            if (worldGroup == null)
                return uiColor;

            return worldGroup.SelectedColor;
        }
    }

    private Color AutoPilotStartingColor
    {
        get 
        {
            if (worldManager == null)
                return autoPilotStartingColor;

            NavigationWorldGroup worldGroup = worldManager.GetWorldGroup();
            if (worldGroup == null)
                return autoPilotStartingColor;

            Color aux = worldGroup.UnselectedColor;
            aux.a = autoPilotStartingColor.a;
            return aux;
        }
    }

    public void BlinkAutoPilot()
    {
        autoPilotText.color = AutoPilotStartingColor;

        //Debug.Log("BlinkAutoPilot()");
        autopilotBlinkSequence = DOTween.Sequence();

        autopilotBlinkSequence.Append(autoPilotText.DOColor(UIColor, 0.6f));
        autopilotBlinkSequence.Append(autoPilotText.DOColor(AutoPilotStartingColor, 1f));

        autopilotBlinkSequence.SetLoops(-1);

        autopilotBlinkSequence.Play();
    }

    public void StopBlinkAutoPilot()
    {
        if (autopilotBlinkSequence == null) return;
        
        autopilotBlinkSequence.Kill();

        autoPilotText.color = AutoPilotStartingColor;
    }

    public void DisplayLandingSign (bool landingOnPlanet)
    {
        landingSignObject.DOFade(1f, 0.6f);

        TextMeshProUGUI textDisplay = landingOnPlanet ? planetLandingText : portalLandingText;
        planetLandingText.enabled = landingOnPlanet;
        portalLandingText.enabled = !landingOnPlanet;

        var s = DOTween.Sequence();

        s.Append(textDisplay.DOFade(0.5f, 0.75f));
        s.Append(textDisplay.DOFade(0.85f, 0.75f));

        s.SetLoops(-1);
    }
    
    public void HideLandingSign()
    {
        landingSignObject.DOFade(0f, 0.1f);
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
