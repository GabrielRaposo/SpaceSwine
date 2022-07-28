using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] TransitionFadeOut transitionFadeOut;
    
    public static bool OnTransition;
    static SceneTransition Instance;

    public enum TransitionType { WhiteFade, SafetyToDanger, DangerToSafety }

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
        LoadScene(currentScene, TransitionType.WhiteFade);
    }

    private void LocalLoadScene(int index, TransitionType transitionType)
    {
        switch (transitionType)
        {
            case TransitionType.SafetyToDanger:
            case TransitionType.DangerToSafety:

            case TransitionType.WhiteFade:
                transitionFadeOut.CallTransition(index);
                break;
        }
        //StartCoroutine( TransitionCoroutine(index) );
    }
}

public enum BuildIndex
{
    Splash = 0,
    Title = 1,
    Ship = 2,
    World1Exploration = 3,
    World1Danger = 4,
    World2Exploration = 5,
    World2Danger = 6,
    World3Exploration = 7,
    World3Danger = 8,
    MinigameJumper = 9,
    World0Exploration = 10,
    TestExplorationStage = 11,
    TestDangerStage = 12,
    NavigationScene = 13
}
