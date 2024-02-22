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

    [Header("Danger Data")]
    [SerializeField] bool toDangerZone;
    [SerializeField] SceneReference shipSceneReference;

    [Header("Completion Feedback")]
    [SerializeField] private StoryEventScriptableObject completionStoryEvent;
    [SerializeField] private SpriteRenderer[] completionDisplays;
    
    [Header("Notification References")]
    [SerializeField] string notificationID;
    [SerializeField] GameObject exclamationIcon;

    public UnityAction OnSelectAction;

    protected override void OnEnable()
    {
        base.OnEnable();

        interactAction += CallShipAnimation;

        if (exclamationIcon != null)
            exclamationIcon.gameObject.SetActive(false);

        CallDependentAction ( SetCompletionDisplay );
    }

    private void CallShipAnimation (NavigationShip ship)
    {
        if (!ship)
            return;

        NavigationShipLandAnimation landAnimation = ship.GetComponent<NavigationShipLandAnimation>();
        if (!landAnimation)
            return;

        {
            //sprite.color = Color.white;
        }

        landAnimation.Call( this, selector.transform, CloseAndSetScene );
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

        for (int i = 0; i < completionDisplays.Length; i++)
        {
            SpriteRenderer sr = completionDisplays[i];
            sr.enabled = StoryEventsManager.IsComplete (completionStoryEvent);

            if (worldGroup)
                sr.color = (i == 0) ? worldGroup.SelectedColor : worldGroup.BackgroundColor; 
            
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
            InteractablePortal.PreCallSetups(shipSceneReference.ScenePath);

        NavigationSceneManager.Instance.CloseAndSetScene( scene.ScenePath, callDangerTransition: toDangerZone );
    }
}
