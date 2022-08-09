using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KeychainInputBanner : MonoBehaviour
{
    [SerializeField] Vector2 startingPos;
    [SerializeField] Vector2 targetPos;
    [SerializeField] Vector2 punchDirection;
    [SerializeField] float duration;

    [Space(10)]
    [SerializeField] RectTransform labelRectTransform;

    bool showing;

    Sequence s;

    void Start()
    {
        if (labelRectTransform)
            labelRectTransform.anchoredPosition = startingPos;
    }

    public void Show()
    {
        if (showing) return;
        showing = true;

        if (s != null)
            s.Kill();
        s = DOTween.Sequence();

        s.AppendInterval(1.0f);
        s.Append( labelRectTransform.DOAnchorPos(targetPos, duration).SetEase(Ease.OutCirc) );
        s.SetUpdate(isIndependentUpdate: true);

        s.OnComplete( SetIdle );
    }

    private void SetIdle() 
    {
        if (s != null)
            s.Kill();
        s = DOTween.Sequence();

        labelRectTransform.anchoredPosition = targetPos;

        s.Append( labelRectTransform.DOPunchAnchorPos(punchDirection, duration: .5f, vibrato: 0) );
        s.SetLoops(-1);
        s.SetUpdate(isIndependentUpdate: true);
    }

    public void Hide()
    {
        if (!showing) return;
        showing = false;

        if (s != null)
            s.Kill();
        s = DOTween.Sequence();

        s.Append( labelRectTransform.DOAnchorPos(startingPos, duration).SetEase(Ease.OutCirc) );
        s.SetUpdate(isIndependentUpdate: true);
    }

}
