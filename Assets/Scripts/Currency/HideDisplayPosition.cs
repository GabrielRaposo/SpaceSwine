using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HideDisplayPosition : MonoBehaviour
{
    [SerializeField] int hiddenPosition;
    [SerializeField] int shownPosition;
    
    [Space(5)]

    [SerializeField] float showDuration;
    [SerializeField] float holdDuration;
    [SerializeField] float hideDuration;

    bool showing;
    float holdCount;

    RectTransform rect;
    Sequence mainSequence;

    void OnEnable()
    {
        rect = GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(hiddenPosition, 0);
    }

    public void Show()
    {
        holdCount = holdDuration; 

        if (showing)
            return;

        showing = true;

        if (!rect)
            rect = GetComponent<RectTransform>();

        if (mainSequence != null)
            mainSequence.Kill();

        mainSequence = DOTween.Sequence();
        mainSequence.Append( rect.DOAnchorPosX(shownPosition, showDuration) );
        mainSequence.OnComplete
        (
            () => 
            {
                holdCount = holdDuration; 
            }
        );
    }

    private void FixedUpdate() 
    {
        if (!showing)
            return;

        holdCount -= Time.fixedDeltaTime;
        if (holdCount <= 0)
        {
            Hide();
        }
    }

    public void Hide()
    {
        if (!showing)
            return;

        showing = false;

        if (!rect)
            rect = GetComponent<RectTransform>();

        if (mainSequence != null)
            mainSequence.Kill();

        mainSequence = DOTween.Sequence();
        mainSequence.Append( rect.DOAnchorPosX(hiddenPosition, hideDuration) );
    }
}
