using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvas : MonoBehaviour
{
    [SerializeField] float duration;

    CanvasGroup canvasGroup;
    Sequence sequence;

    static FadeCanvas Instance;

    private void Awake() 
    {
        Instance = this;
        
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public static void Call (UnityAction midFadeAction, UnityAction afterFadeAction)
    {
        if (Instance == null)
        {
            midFadeAction?.Invoke();
            afterFadeAction?.Invoke();
            return;
        }

        Instance.LocalCall (midFadeAction, afterFadeAction);
    }

    private void LocalCall (UnityAction midFadeAction, UnityAction afterFadeAction)
    {
        if (sequence != null)
            sequence.Kill();

        canvasGroup.alpha = 0;

        sequence = DOTween.Sequence();

        sequence.Append ( canvasGroup.DOFade(1f, duration/2f) );
        sequence.AppendCallback( midFadeAction.Invoke );

        sequence.Append ( canvasGroup.DOFade(0f, duration/2f) );
        sequence.OnComplete ( afterFadeAction.Invoke );

        sequence.SetUpdate(isIndependentUpdate: true);
    }
}
