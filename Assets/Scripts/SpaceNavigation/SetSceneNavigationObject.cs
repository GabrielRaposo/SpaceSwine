using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using DevLocker.Utils;

public class SetSceneNavigationObject : NavigationObject
{
    [Header("     ")]
    public SceneReference scene;
    public SpriteRenderer selector;

    [SerializeField] private Transform debugShip;
    [SerializeField] private GameObject navigationDot;
    [SerializeField] private Transform dotsParent;

    [Header("Danger Data")]
    [SerializeField] bool toDangerZone;
    [SerializeField] SceneReference shipSceneReference;
    [SerializeField] RoundSessionData data;

    [Header("Completion Feedback")]
    [SerializeField] private StoryEventScriptableObject completionStoryEvent;
    [SerializeField] private SpriteRenderer[] completionDisplays;
    
    [Header("Notification References")]
    [SerializeField] string notificationID;
    [SerializeField] GameObject exclamationIcon;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event OnHoverAKEvent;
    [SerializeField] AK.Wwise.Event OnSelectAKEvent; 
    [SerializeField] AK.Wwise.Event MakePathAKEvent;
    [SerializeField] AK.Wwise.Event OnReachDestinationAKEvent;
    
    private float p2Lenght = 1.5f;
    private float p3Lenght = 0.6f;
    private Curve animationBezier;

    private Vector2 p2Debug;
    private Vector2 p3Debug;

    private NavigationSceneManager navSceneManager;

    public UnityAction OnSelectAction;

    protected override void OnEnable()
    {
        base.OnEnable();

        interactAction += CallShipAnimation;
        navSceneManager = FindObjectOfType<NavigationSceneManager>();

        if (exclamationIcon != null)
            exclamationIcon.gameObject.SetActive(false);

        CallDependentAction ( SetCompletionDisplay );
    }

    private void CallShipAnimation (NavigationShip ship)
    {

    }

    private void SetCompletionDisplay()
    {
        if (completionDisplays == null)
            return;

        if (completionStoryEvent == null)
        {
            foreach (SpriteRenderer sr in completionDisplays)
                sr.enabled = false;

            return;
        }

        foreach (SpriteRenderer sr in completionDisplays)
        {
            sr.enabled = StoryEventsManager.IsComplete (completionStoryEvent);
            sr.gameObject.SetActive(false);
        }
    }

    public override void SetNotificationIcon()
    {
        if (exclamationIcon == null)
            return;

        bool value = false;

        if (notificationID != string.Empty)
            value = UINotificationManager.Check( notificationID );

        exclamationIcon.SetActive (value);
    }

    public override void OnSelect()
    {
        base.OnSelect();

        foreach (SpriteRenderer sr in completionDisplays)
            sr.gameObject.SetActive(true);

        selector.enabled = true;

        if (OnHoverAKEvent != null)
            OnHoverAKEvent.Post(gameObject);

        if (OnSelectAction != null)
            OnSelectAction.Invoke();
    }

    public override void OnDeselect()
    {
        base.OnDeselect();

        foreach (SpriteRenderer sr in completionDisplays)
            sr.gameObject.SetActive(false);

        selector.enabled = false;
    }

    protected virtual void CloseAndSetScene()
    {
        if (NavigationSceneManager.Instance == null)
        {
            Debug.Log("NAV SCENE MANAGER NOT FOUND");
            return;
        }

        if (notificationID != string.Empty)
            UINotificationManager.Use (notificationID);

        if (toDangerZone) 
            InteractablePortal.PreCallSetups(shipSceneReference.ScenePath, data);

        NavigationSceneManager.Instance.CloseAndSetScene( scene.ScenePath, callDangerTransition: toDangerZone );
    }

    private void OnDrawGizmos()
    {
        if(debugShip == null) return;
        
        for (float i = 0f; i < 1f; i+=0.03f)
        {
            Gizmos.color = Color.yellow;
            var from = animationBezier.GetPoint(i);
            var to = animationBezier.GetPoint(i + 0.03f);
            Gizmos.DrawLine(from,to);
        }
        
        Gizmos.color = Color.cyan;
        
        Gizmos.DrawWireSphere(p2Debug, 0.1f);
        Gizmos.DrawWireSphere(p3Debug, 0.1f);
    }
}
