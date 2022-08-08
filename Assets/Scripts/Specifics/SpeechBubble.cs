using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] float duration;    
    [SerializeField] Transform visualComponent;

    Sequence sequence;
    Animator animator;

    private void Start() 
    {
        animator = GetComponent<Animator>();

        if (!visualComponent)
            return;

        visualComponent.transform.localScale = Vector2.zero;
    }

    public void Show()
    {
        if (!visualComponent)
            return;

        if (sequence != null)
            sequence.Kill();

        visualComponent.localScale = Vector2.zero;

        sequence = DOTween.Sequence();
        sequence.Append( visualComponent.DOScale(endValue: 1.1f, duration) );
        sequence.Append( visualComponent.DOScale(endValue: 1.0f, duration/10f) );
    }

    public void Hide()
    {
        if (!visualComponent)
            return;

        if (sequence != null)
            sequence.Kill();

        visualComponent.localScale = Vector2.one;

        sequence = DOTween.Sequence();
        sequence.Append( visualComponent.DOScale(endValue: 0, duration) );
    }

    public void SetAnimationState (bool state)
    {
        animator.SetInteger("State", state ? -1 : 1);
    }
}
