using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class RoundTransition : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] float duration;

    Sequence mainSequence;
    
    public static bool OnTransition;
    static RoundTransition Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy (gameObject);
            return;
        }

        Instance = this;
    }

    public static void Call(UnityAction action)
    {
        if (Instance)
        {
            Instance.LocalCall(action);
        }
        else
        {
            action?.Invoke();
        }
    }

    private void LocalCall(UnityAction action)
    {
        StartCoroutine( TransitionCoroutine(action) );
    }

    private IEnumerator TransitionCoroutine(UnityAction action)
    {
        OnTransition = true;
        bool done = false;

        if (!fillImage)
            yield break;

        if (mainSequence != null)
            mainSequence.Kill();

        fillImage.color = new Color(0,0,0,0);

        // Mostra transição
        mainSequence = DOTween.Sequence();
        mainSequence.Append( fillImage.DOFade(1, duration) );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);

        yield return new WaitUntil( () => done );
        done = false;

        // Chamada de action 
        action?.Invoke();
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
