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
        PauseSystem.OnUseTeleporterAction += OnUseTeleporterAction;

        Debug.Log("TO-DO: Checking UI Notification on Update");
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
        if (!shown)
            return;

        gameObject.SetActive(false);
    }

    private void OnUseTeleporterAction()
    {
        if (!shown)
            return;

        UINotificationManager.Use(notificationID);
    }

    private void OnDisable() 
    {
        if (string.IsNullOrEmpty(notificationID))
            return;
 
        PauseSystem.OnPauseAction -= OnPauseAction;
        PauseSystem.OnUseTeleporterAction -= OnUseTeleporterAction;
    }
}
