using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TransitionFadeOut : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] float duration;

    Sequence mainSequence;

    void Start()
    {
        
    }

    public void CallTransition(int index)
    {
        StartCoroutine( TransitionCoroutine(index) );
    }

    private IEnumerator TransitionCoroutine(int index)
    {
        SceneTransition.OnTransition = true;
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

        SceneTransition.OnTransition = false;
    }
}
