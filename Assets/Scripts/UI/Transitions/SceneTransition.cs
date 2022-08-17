using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] TransitionFadeOut transitionFadeOut;
    [SerializeField] TransitionSafetyToDanger transitionSafetyToDanger;
    
    public static bool OnTransition;
    static SceneTransition Instance;

    public enum TransitionType { BlackFade, SafetyToDanger, DangerToSafety }

    private void Awake()
    {
        if (Instance)
        {
            Destroy (gameObject);
            return;
        }

        Instance = this;
    }

    public static void LoadScene(int index, TransitionType transitionType)
    {
        SaveManager.Save();

        if (Instance)
        {
            Instance.LocalLoadScene(index, transitionType);
        }
        else
        {
            SceneManager.LoadScene(index);
        }
    }

    public static void ReloadScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        LoadScene(currentScene, TransitionType.BlackFade);
    }

    private void LocalLoadScene(int index, TransitionType transitionType)
    {
        switch (transitionType)
        {
            case TransitionType.SafetyToDanger:
                transitionSafetyToDanger.CallTransition(index, safetyToDanger: true);
                break;

            case TransitionType.DangerToSafety:
                transitionSafetyToDanger.CallTransition(index, safetyToDanger: false);
                break;

            case TransitionType.BlackFade:
                transitionFadeOut.CallTransition(index);
                break;
        }
    }
}

public enum BuildIndex
{
    Splash                  = 0,
    Title                   = 1,
    Ship                    = 2,
    World1Exploration       = 3,
    World1Danger            = 4,
    World2Exploration       = 5,
    World2Danger            = 6,
    World3Exploration       = 7,
    World3Danger            = 8,
    MinigameJumper          = 9,
    World0Exploration       = 10,
    TestExplorationStage    = 11,
    TestDangerStage         = 12,
    NavigationScene         = 13,
    WorldXExploration       = 14
}
