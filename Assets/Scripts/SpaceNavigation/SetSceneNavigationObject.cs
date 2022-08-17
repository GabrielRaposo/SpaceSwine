using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SetSceneNavigationObject : NavigationObject
{
    [Header("     ")]
    public BuildIndex scene = BuildIndex.World1Exploration;
    public SpriteRenderer selector;

    [SerializeField] private Transform debugShip;
    private float p2Lenght = 1.5f;
    private float p3Lenght = 0.6f;
    private Curve animationBezier;
    
    [SerializeField] private GameObject navigationDot;

    private Vector2 p2Debug;
    private Vector2 p3Debug;

    private NavigationSceneManager navSceneManager;

    [SerializeField] private Transform dotsParent;
    [SerializeField] private AK.Wwise.Event sound; 
    
    [Header("Audio")]
    [SerializeField] AK.Wwise.Event OnSelectAKEvent;

    public UnityAction OnSelectAction;

    private void OnEnable()
    {
        interactAction += ShipAnimation;

        navSceneManager = FindObjectOfType<NavigationSceneManager>();
    }

    public override void OnSelect()
    {
        base.OnSelect();
        selector.enabled = true;

        if (OnSelectAKEvent != null)
            OnSelectAKEvent.Post(gameObject);

        if (OnSelectAction != null)
            OnSelectAction.Invoke();
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
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

        sound.Post(gameObject);
        
        //Feedback de seleção e UI de autopilot         
        sequence.Append(selector.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.22f).OnComplete(()=>navSceneManager.BlinkAutoPilot()));
        sequence.AppendInterval(0.1f);
        
        float x = 0f;
        float shipStartRotation = navigationShip.spritesTransform.eulerAngles.z + 90f;
        Vector2 startPos = navigationShip.transform.position;

        //Animação curta ou longa
        if (Vector2.Distance(startPos, transform.position) < 0.7f)
        {
            var angle = Mathg.AngleOfTheLineBetweenTwoPoints(new Vector2(transform.position.x, transform.position.y), startPos)-180f;
            sequence.Append(
                navigationShip.spritesTransform.DORotate(new Vector3(0f,0f, angle),1.3f));
            sequence.Join(navigationShip.transform.DOMove(transform.position, 1.3f).OnComplete(
                () =>
                {
                    //Mostra UI de aterrisagem no final
                    navSceneManager.StopBlinkAutoPilot();
                    navSceneManager.DisplayLandingSign();    
                }));
        }
        else
        {
            SetCurve(shipStartRotation, startPos, transform.position);
        
            sequence.Append(DrawDots(.3f, 10, shipStartRotation, startPos, transform.position));

            sequence.AppendInterval(0.35f);
        
            //Tween da movimentação e rotação
            sequence.Append(DOVirtual.Float(0f, 1f, 2.75f, value =>
                {
                    navigationShip.transform.position = animationBezier.GetPoint(value);
                    navigationShip.spritesTransform.eulerAngles = new Vector3(0f, 0f,
                        animationBezier.GetRotationAtPoint(value));
                }).SetEase(Ease.OutFlash)
                .OnComplete(() =>
                {
                    //Mostra UI de aterrisagem no final
                    navSceneManager.StopBlinkAutoPilot();
                    navSceneManager.DisplayLandingSign();
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

        NavigationSceneManager.Instance.CloseAndSetScene((int)scene);
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

        float c = count;
        
        for (int i = 0; i < count; i++)
        {
            int index = i;
            s.Append(DOVirtual.DelayedCall(duration / c, () =>
            {
                var pos = animationBezier.GetNormalizedPoint(index / c);
                var dot = dotsParent.GetChild(index);
                dot.position = pos;
                sound.Post(gameObject);
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
