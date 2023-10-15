using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SetWorldNavigationObject : NavigationObject
{
    [Header("Notification References")]
    [SerializeField] string notificationID;
    [SerializeField] GameObject exclamationIcon;

    [Header("World")]
    [SerializeField] SpriteRenderer selector;
    [SerializeField] int indexChange;

    public UnityAction OnSelectAction;

    protected override void OnEnable()
    {
        base.OnEnable();

        interactAction += CallShipAnimation;

        if (exclamationIcon != null)
            exclamationIcon.gameObject.SetActive(false);

        //CallDependentAction ( SetCompletionDisplay );
    }

    private void CallShipAnimation (NavigationShip ship)
    {
        if (!ship)
            return;

        NavigationShipLandAnimation landAnimation = ship.GetComponent<NavigationShipLandAnimation>();
        if (!landAnimation)
            return;

        {
            sprite.color = Color.white;
        }

        landAnimation.Call( this, selector.transform, SetWorld );
    }

    protected virtual void SetWorld()
    {
        if (NavigationSceneManager.Instance == null)
        {
            Debug.Log("NAV SCENE MANAGER NOT FOUND");
            return;
        }

        //if (notificationID != string.Empty)
        //    UINotificationManager.Use (notificationID);

        //if (toDangerZone) 
        //    InteractablePortal.PreCallSetups(shipSceneReference.ScenePath, data);

        //NavigationSceneManager.Instance.CloseAndSetScene( scene.ScenePath, callDangerTransition: toDangerZone );
        NavigationWorldManager.Instance.ChangeWorld(indexChange);
    }

    public override void OnSelect()
    {
        base.OnSelect();

        selector.enabled = true;

        if (OnSelectAction != null)
            OnSelectAction.Invoke();
    }

    public override void OnDeselect()
    {
        base.OnDeselect();

        selector.enabled = false;
    }
}
