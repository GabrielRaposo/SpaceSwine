using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class NavigationShipLandAnimation : MonoBehaviour
{
    [SerializeField] private Transform dotsParent;

    [Header("Visual References")]
    [SerializeField] SpriteRenderer frontSprite;
    [SerializeField] SpriteRenderer backSprite;
    [SerializeField] ParticleSystem trailPS;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event OnSelectAKEvent; 
    [SerializeField] AK.Wwise.Event MakePathAKEvent;
    [SerializeField] AK.Wwise.Event OnReachDestinationAKEvent;

    //private float p2Lenght = 1.5f;
    //private float p3Lenght = 0.6f;
    private Curve animationBezier;

    private Vector2 p2Debug;
    private Vector2 p3Debug;

    NavigationSceneManager navSceneManager;
    NavigationShip navigationShip;


    void Start()
    {
        navSceneManager = NavigationSceneManager.Instance;
        navigationShip = GetComponent<NavigationShip>();
    }

    public void Call (NavigationObject caller, Transform selector, UnityAction AfterSequenceAction, bool landingOnPlanet)
    {
        if (navigationShip == null)
        {
            AfterSequenceAction();
            return;
        }

        navigationShip.LockControls();

        if (OnSelectAKEvent != null)
            OnSelectAKEvent.Post(gameObject);

        var sequence = DOTween.Sequence();

        // -- Feedback de seleção e UI de autopilot
        if (selector != null)
            sequence.Append(selector.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.22f).OnComplete(() => navSceneManager.BlinkAutoPilot()));
        sequence.AppendInterval(0.1f);

        float shipStartRotation = navigationShip.spritesTransform.eulerAngles.z + 90f;
        Vector2 startPos = navigationShip.transform.position;

        NavigationShipSoundController shipSoundController = navigationShip.GetComponent<NavigationShipSoundController>();

        sequence.AppendCallback(() =>
        {
            if (shipSoundController)
            {
                navigationShip.SetFlightStepsSound(true);
                navigationShip.OverridenControls = Vector2.up;
                navigationShip.OverrideMode = true;
            }
        });

        // -- Animação curta
        if (Vector2.Distance(startPos, caller.transform.position) < 0.7f)
        {
            var angle = Mathg.AngleOfTheLineBetweenTwoPoints(new Vector2(caller.transform.position.x, caller.transform.position.y), startPos) - 180f;
            sequence.AppendCallback(() =>
            {
                navigationShip.SetFlightStepsSound(true);
                if (shipSoundController)
                {
                    this.WaitSeconds(.1f, () => shipSoundController.ReadInput(Vector2.up, intensity: 1.84f));
                }
            });
            sequence.Append(navigationShip.spritesTransform.DORotate(new Vector3(0f, 0f, angle), 1.3f));
            sequence.Join(navigationShip.transform.DOMove(caller.transform.position, 1.3f).OnComplete(
                () =>
                {
                    // -- Mostra UI de aterrisagem no final
                    navSceneManager.StopBlinkAutoPilot();
                    navSceneManager.DisplayLandingSign(landingOnPlanet);

                    navigationShip.SetFlightStepsSound(false);
                    navigationShip.OverridenControls = Vector2.zero;

                    if (OnReachDestinationAKEvent != null)
                        OnReachDestinationAKEvent.Post(gameObject);
                }));
        }
        // -- Animação longa
        else
        {
            SetCurve(shipStartRotation, startPos, caller.transform.position);

            sequence.Append(DrawDots(.3f, 8, shipStartRotation, startPos, caller.transform.position));

            sequence.AppendInterval(0.35f);

            sequence.AppendCallback(() =>
            {
                navigationShip.SetFlightStepsSound(true);
                if (shipSoundController)
                {
                    this.WaitSeconds(.1f, () => shipSoundController.ReadInput(Vector2.up, intensity: 1.84f));
                }
            });

            // -- Tween da movimentação e rotação
            sequence.Append(DOVirtual.Float(0f, 1f, 1.85f, value =>
                {
                    navigationShip.transform.position = animationBezier.GetPoint(value);
                    navigationShip.spritesTransform.eulerAngles = new Vector3(0f, 0f,
                        animationBezier.GetRotationAtPoint(value));
                }).SetEase(Ease.OutFlash)
                .OnComplete(() =>
                {
                    // -- Mostra UI de aterrisagem no final
                    navSceneManager.StopBlinkAutoPilot();
                    navSceneManager.DisplayLandingSign(landingOnPlanet);

                    navigationShip.SetFlightStepsSound(false);
                    navigationShip.OverridenControls = Vector2.zero;

                    if (OnReachDestinationAKEvent != null)
                        OnReachDestinationAKEvent.Post(gameObject);
                })
            );
        }

        sequence.AppendInterval(2.5f);

        //Setta Cena e fecha
        sequence.OnComplete( () => AfterSequenceAction() );
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
            s.Append (DOVirtual.DelayedCall (duration / c, () =>
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

    private void SetCurve (float startRotation, Vector2 startPos, Vector2 endPos)
    {
        float p2Lenght = Random.Range(0.2f, 0.8f);
        float p3Lenght = Random.Range(0.2f, 0.8f);

        float randomAngle2 = Random.Range(-35f, 35f);
        float randomAngle3 = Random.Range(-35f, 35f);
    
        Vector2 p2 = startPos + new Vector2 (Mathg.AngleToDirection(startRotation+randomAngle2).x, Mathg.AngleToDirection(startRotation+randomAngle3).y) * p2Lenght;
        p2Debug = p2;

        float side = (startPos.y < endPos.y) ? -1f : 1f;

        Vector2 p3 = endPos + Vector2.up * p3Lenght * side;
        p3Debug = p3;

        animationBezier = new Curve(startPos, p2, p3, endPos);
    }

    
    public void ClearScreenState()
    {
        if (dotsParent != null)
        {
            for (int i = 0; i < dotsParent.childCount; i++)
            {
                var dot = dotsParent.GetChild (i);
                dot.position = dotsParent.position;
            }
        }

        if (navSceneManager)
        {
            navSceneManager.HideLandingSign();
        }

        if (navigationShip) 
        {
            navigationShip.SavePreviousPosition();
        }
    }

    public void UnlockControls()
    {
        if (navigationShip)
        {
            navigationShip.OverrideMode = false;
            navigationShip.UnlockControls();
        }
    }

    public void JumpToPosition (Vector3 position)
    {
        transform.position = position;
    }

    public void UpdateColors (Color selectedColor, Color unselectedColor, Color backColor)
    {
        if (frontSprite != null)
            frontSprite.color = selectedColor;

        if (backSprite != null)
            backSprite.color = backColor;

        if (trailPS != null)
        {
            ParticleSystem.MainModule mainModule = trailPS.main;
            mainModule.startColor = selectedColor;
        }

        // -- Dots visuals
        if (dotsParent != null)
        {
            SpriteRenderer[] dotSprites = dotsParent.GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer dotSprite in dotSprites) 
            {
                dotSprite.color = unselectedColor;
            }
        }
    }

    private void OnDrawGizmos()
    {       
        return;

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
