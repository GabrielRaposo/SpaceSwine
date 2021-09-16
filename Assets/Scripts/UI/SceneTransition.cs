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
        Debug.Log("Local Load Scene");
        StartCoroutine( TransitionCoroutine(index) );
    }

    private IEnumerator TransitionCoroutine(int index)
    {
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

        Debug.Log ("Transition ended");
    }
}

public enum BuildIndex
{
    // splash screen = 0
    Title = 0,
    TestStage = 1
}
