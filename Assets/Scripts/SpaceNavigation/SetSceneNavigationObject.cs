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

        interactAction += ShipAnimation;
        navSceneManager = FindObjectOfType<NavigationSceneManager>();

        if (exclamationIcon != null)
            exclamationIcon.gameObject.SetActive(false);

        CallDependentAction ( SetCompletionDisplay );
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

    private void ShipAnimation(NavigationShip navigationShip)
    {
        if (navigationShip == null)
        {
            CloseAndSetScene();
            return;
        }

        navigationShip.LockControls();

        var sequence = DOTween.Sequence();

        sprite.color = Color.white;

        if (OnSelectAKEvent != null)
            OnSelectAKEvent.Post(gameObject);
        
        // -- Feedback de seleção e UI de autopilot         
        sequence.Append(selector.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.22f).OnComplete(() => navSceneManager.BlinkAutoPilot()));
        sequence.AppendInterval(0.1f);
        
        float x = 0f;
        float shipStartRotation = navigationShip.spritesTransform.eulerAngles.z + 90f;
        Vector2 startPos = navigationShip.transform.position;

        NavigationShipSoundController shipSoundController = navigationShip.GetComponent<NavigationShipSoundController>();
        
        sequence.AppendCallback( () => 
        {
            if (shipSoundController)
            {
                navigationShip.SetFlightStepsSound(true);
                navigationShip.OverridenControls = Vector2.up;
                navigationShip.OverrideMode = true;
            }
        } );

        // -- Animação curta
        if (Vector2.Distance(startPos, transform.position) < 0.7f)
        {
            var angle = Mathg.AngleOfTheLineBetweenTwoPoints(new Vector2(transform.position.x, transform.position.y), startPos) - 180f;
            sequence.AppendCallback( () => 
            {
                navigationShip.SetFlightStepsSound(true);
                if (shipSoundController)
                {
                    this.WaitSeconds(.1f, () => shipSoundController.ReadInput(Vector2.up, intensity: 1.84f) );
                }
            });
            sequence.Append( navigationShip.spritesTransform.DORotate(new Vector3(0f,0f, angle),1.3f));
            sequence.Join(navigationShip.transform.DOMove(transform.position, 1.3f).OnComplete(
                () =>
                {
                    // -- Mostra UI de aterrisagem no final
                    navSceneManager.StopBlinkAutoPilot();
                    navSceneManager.DisplayLandingSign();    

                    navigationShip.SetFlightStepsSound(false);
                    navigationShip.OverridenControls = Vector2.zero;
                    
                    if (OnReachDestinationAKEvent != null)
                        OnReachDestinationAKEvent.Post(gameObject);
                }));
        }
        // -- Animação longa
        else
        {
            SetCurve(shipStartRotation, startPos, transform.position);
        
            sequence.Append(DrawDots(.3f, 10, shipStartRotation, startPos, transform.position));

            sequence.AppendInterval(0.35f);
        
            sequence.AppendCallback( () => 
            {
                navigationShip.SetFlightStepsSound(true);
                if (shipSoundController)
                {
                    this.WaitSeconds(.1f, () => shipSoundController.ReadInput(Vector2.up, intensity: 1.84f) );
                }
            });

            // -- Tween da movimentação e rotação
            sequence.Append(DOVirtual.Float(0f, 1f, 2.75f, value =>
                {
                    navigationShip.transform.position = animationBezier.GetPoint(value);
                    navigationShip.spritesTransform.eulerAngles = new Vector3(0f, 0f,
                        animationBezier.GetRotationAtPoint(value));
                }).SetEase(Ease.OutFlash)
                .OnComplete(() =>
                {
                    // -- Mostra UI de aterrisagem no final
                    navSceneManager.StopBlinkAutoPilot();
                    navSceneManager.DisplayLandingSign();

                    navigationShip.SetFlightStepsSound(false);
                    navigationShip.OverridenControls = Vector2.zero;

                    if (OnReachDestinationAKEvent != null)
                        OnReachDestinationAKEvent.Post(gameObject);
                })
            );    
        }
        
        sequence.AppendInterval(2.5f);

        //Setta Cena e fecha
        sequence.OnComplete(CloseAndSetScene);

    }
    
    private void CloseAndSetScene()
    {
        if (NavigationSceneManager.Instance == null)
        {
            Debug.Log("NAV SCENE MANAGER NOT FOUND");
            return;
        }

        if (notificationID != string.Empty)
            UINotificationManager.Use (notificationID);

        NavigationSceneManager.Instance.CloseAndSetScene( scene.ScenePath );
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

    private Tween DrawDots(float duration, int count, float startRotation, Vector2 startPos, Vector2 endPos)
    {
        var s = DOTween.Sequence();

        if (OnSelectAKEvent != null)
            OnSelectAKEvent.Post(gameObject);

        float c = count;
        
        for (int i = 0; i < count; i++)
        {
            int index = i;
            s.Append(DOVirtual.DelayedCall(duration / c, () =>
            {
                var pos = animationBezier.GetNormalizedPoint(index / c);
                var dot = dotsParent.GetChild(index);
                dot.position = pos;
                
                if (MakePathAKEvent != null)
                    MakePathAKEvent.Post(gameObject);
                
            }));
        }

        return s;
    }

    private void SetCurve(float startRotation, Vector2 startPos, Vector2 endPos)
    {
        Vector2 p2 = startPos + new Vector2(Mathg.AngleToDirection(startRotation).x,Mathg.AngleToDirection(startRotation).y) * p2Lenght;
        p2Debug = p2;
        
        float side = (startPos.y < endPos.y) ? -1f : 1f;
        
        Vector2 p3 = endPos + Vector2.up * p3Lenght * side;
        p3Debug = p3;
        
        animationBezier = new Curve(startPos, p2, p3, endPos);
    }
    
}
