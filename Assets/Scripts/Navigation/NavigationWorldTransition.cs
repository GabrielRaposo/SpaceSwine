using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class NavigationWorldTransition : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float duration;
    [SerializeField] float hold;

    Sequence s;

    public void CallTransition (UnityAction midAction, UnityAction afterAction)
    {
        if (!canvasGroup)
            return;

        if (s == null)
            s.Kill();

        {
            canvasGroup.alpha = 0;
        }

        s = DOTween.Sequence();
        s.Append( canvasGroup.DOFade(1, duration) );
        s.OnComplete
        ( 
            () => {
                midAction.Invoke();
                OutTransition(afterAction); 
            }
        );
    }

    private void OutTransition(UnityAction afterAction)
    {
        s = DOTween.Sequence();
        s.AppendInterval( hold );
        s.Append( canvasGroup.DOFade(0, duration) );
        s.AppendCallback( afterAction.Invoke );
    }
}
