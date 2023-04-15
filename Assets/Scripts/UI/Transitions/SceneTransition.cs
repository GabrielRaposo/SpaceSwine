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

    public static void LoadScene(string path, TransitionType transitionType)
    {
        SaveManager.Save();

        if (Instance)
        {
            Instance.LocalLoadScene(path, transitionType);
        }
        else
        {
            SceneManager.LoadScene(path);
        }
    }

    public static void ReloadScene()
    {
        string currentScene = SceneManager.GetActiveScene().path;
        LoadScene(currentScene, TransitionType.BlackFade);
    }

    private void LocalLoadScene (string path, TransitionType transitionType)
    {
        switch (transitionType)
        {
            case TransitionType.SafetyToDanger:
                transitionSafetyToDanger.CallTransition(path, safetyToDanger: true);
                break;

            case TransitionType.DangerToSafety:
                transitionSafetyToDanger.CallTransition(path, safetyToDanger: false);
                break;

            case TransitionType.BlackFade:
                transitionFadeOut.CallTransition(path);
                break;
        }
    }
}

public enum BuildIndex
{
    Splash = 0,
    Title = 1,
    Ship = 2,
    NavigationScene = 3,
    MakeABeat = 4
}
