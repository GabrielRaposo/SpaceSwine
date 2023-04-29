using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SetTextMeshOnUINotification : MonoBehaviour
{
    [SerializeField] string notificationID;

    CanvasGroup canvasGroup;
    Sequence s;

    bool shown;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    private void OnEnable() 
    {
        if (string.IsNullOrEmpty(notificationID))
            return;
 
        PauseSystem.OnPauseAction += OnPauseAction;
    }

    private void Update() 
    {
        if (shown)
            return;

        if (!UINotificationManager.Check(notificationID))
            return;

        ShowCanvas();
    }

    private void ShowCanvas ()
    {
        shown = true;

        s = DOTween.Sequence();
        s.Append( canvasGroup.DOFade(endValue: 1, duration: 1.0f) );
    }

    private void OnPauseAction()
    {
        UINotificationManager.Use(notificationID);
        gameObject.SetActive(false);
    }

    private void OnDisable() 
    {
        if (string.IsNullOrEmpty(notificationID))
            return;
 
        PauseSystem.OnPauseAction -= OnPauseAction;
    }
}
