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

    [SerializeField] ParticleSystem idlePS;

    public UnityAction OnSelectAction;

    protected override void OnEnable()
    {
        base.OnEnable();

        interactAction += CallShipAnimation;

        if (selector != null)
            selector.enabled = false;

        if (exclamationIcon != null)
            exclamationIcon.gameObject.SetActive(false);

        //CallDependentAction ( SetCompletionDisplay );
        SetParticleColor();
    }

    private void SetParticleColor()
    {
        if (idlePS == null)
            return;

        NavigationWorldGroup navigationWorldGroup = GetComponentInParent<NavigationWorldGroup>();
        if (navigationWorldGroup == null)
            return;

        ParticleSystem.MainModule mainModule = idlePS.main;
        mainModule.startColor = navigationWorldGroup.SelectedColor;
    }

    private void CallShipAnimation (NavigationShip ship)
    {
        if (!ship)
            return;

        this.ship = ship;

        NavigationShipLandAnimation landAnimation = ship.GetComponent<NavigationShipLandAnimation>();
        if (!landAnimation)
            return;

        {
            //sprite.color = Color.white;
        }

        landAnimation.Call( this, selector.transform, SetWorld, landingOnPlanet: false );
    }

    protected virtual void SetWorld()
    {
        if (NavigationSceneManager.Instance == null)
        {
            Debug.Log("NAV SCENE MANAGER NOT FOUND");
            return;
        }

        if (notificationID != string.Empty)
            UINotificationManager.Use(notificationID);

        NavigationShipLandAnimation landAnimation = ship.GetComponent<NavigationShipLandAnimation>();
        if (!landAnimation)
            return;
        
        NavigationWorldManager.Instance.ChangeWorld (indexChange, landAnimation);
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
