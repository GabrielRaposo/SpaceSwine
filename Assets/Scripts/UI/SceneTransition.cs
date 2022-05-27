using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] float duration;

    Sequence mainSequence;
    
    public static bool OnTransition;
    static SceneTransition Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy (gameObject);
            return;
        }

        Instance = this;
    }

    public static void LoadScene(int index)
    {
        SaveManager.Save();

        if (Instance)
        {
            Instance.LocalLoadScene(index);
        }
        else
        {
            SceneManager.LoadScene(index);
        }
    }

    public static void ReloadScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        LoadScene(currentScene);
    }

    private void LocalLoadScene(int index)
    {
        StartCoroutine( TransitionCoroutine(index) );
    }

    private IEnumerator TransitionCoroutine(int index)
    {
        OnTransition = true;
        bool done = false;

        if (!fillImage)
            yield break;

        if (mainSequence != null)
            mainSequence.Kill();

        fillImage.color = new Color(1,1,1,0);

        // Mostra transição
        mainSequence = DOTween.Sequence();
        mainSequence.Append( fillImage.DOFade(1, duration) );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);

        yield return new WaitUntil( () => done );
        done = false;

        DOTween.KillAll();

        // Carrega cena
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);

        while (!asyncOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();

        // Some com transição
        mainSequence = DOTween.Sequence();
        mainSequence.Append( fillImage.DOFade(0, duration) );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);
        
        yield return new WaitUntil( () => done );

        OnTransition = false;
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
    TestDangerStage = 12
}
