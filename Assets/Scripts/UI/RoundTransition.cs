using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class RoundTransition : MonoBehaviour
{
    [SerializeField] RectTransform fillAnchor;

    [Header("Values")]
    [SerializeField] float duration;
    [SerializeField] float topY;
    [SerializeField] float botY;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event transitionAKEvent;

    Sequence mainSequence;
    CanvasGroup canvasGroup;
    
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

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
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

        if (mainSequence != null)
            mainSequence.Kill();

        fillAnchor.MoveY(botY);
        canvasGroup.alpha = 1;

        if (transitionAKEvent != null)
            transitionAKEvent.Post(gameObject);

        // -- Mostra transição
        mainSequence = DOTween.Sequence();
        mainSequence.Append
        ( 
            DOVirtual.Float
            (
                from: botY, to: 0, duration, 
                (f) => fillAnchor.MoveY(f)
            ).SetEase(Ease.Linear)
        );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);

        yield return new WaitUntil( () => done );
        done = false;

        // -- Chamada de action 
        action?.Invoke();
        yield return new WaitForEndOfFrame();

        // -- Some com transição
        mainSequence = DOTween.Sequence();
        mainSequence.Append
        ( 
            DOVirtual.Float
            (
                from: 0, to: topY, duration, 
                (f) => fillAnchor.MoveY(f)
            ).SetEase(Ease.Linear)
        );
        mainSequence.OnComplete( () => done = true );
        mainSequence.SetUpdate(isIndependentUpdate: true);
        
        yield return new WaitUntil( () => done );

        OnTransition = false;
    }
}
