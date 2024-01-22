using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NavigationNewLocationDisplay : MonoBehaviour
{
    void Start()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        SetSceneNavigationObject navigationObject = GetComponentInParent<SetSceneNavigationObject>();

        if (!canvasGroup || !navigationObject)
            return;

        navigationObject.OnSelectAction += () => { canvasGroup.alpha = 0; };

        RectTransform rt = GetComponent<RectTransform>();
        if (!rt)
            return;

        DOVirtual.Float(from: .9f, to: 1.0f, .9f, f => rt.MoveY(f) )
            .SetLoops(-1);
    }
}
