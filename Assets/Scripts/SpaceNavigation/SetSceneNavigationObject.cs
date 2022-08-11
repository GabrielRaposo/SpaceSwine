using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SetSceneNavigationObject : NavigationObject
{
    public BuildIndex scene = BuildIndex.World1Exploration;
    public SpriteRenderer selector;

    [SerializeField] private Transform debugShip;
    private float p2Lenght = 1.5f;
    private float p3Lenght = 0.6f;

    private Vector2 p2Debug;
    private Vector2 p3Debug;
    
    private void OnEnable()
    {
        interactAction += ShipAnimation;
    }

    public override void OnSelect()
    {
        base.OnSelect();
        selector.enabled = true;
    }

    public override void OnDisselect()
    {
        base.OnDisselect();
        selector.enabled = false;
    }

    private void ShipAnimation(NavigationShip navigationShip)
    {
        if (navigationShip == null)
        {
            CloseAndSetScene();
            return;
        }

        navigationShip.lockControls = true;

        var sequence = DOTween.Sequence();

        sequence.Append(selector.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.25f));
        
        float x = 0f;
        float shipStartRotation = navigationShip.spritesTransform.eulerAngles.z + 90f;
        Vector2 startPos = navigationShip.transform.position;

        sequence.Append(DOVirtual.Float(0f, 1f, 2.75f, value =>
        {
            navigationShip.transform.position = AnimationCurvePosition(value, shipStartRotation, startPos, transform.position);
            navigationShip.spritesTransform.transform.eulerAngles = new Vector3(0f,0f,
                AnimationCurveRotation(value, shipStartRotation, startPos, transform.position));
        }).SetEase(Ease.OutFlash)
        );
        
        float rotation;

        Debug.Log("euler z:" + navigationShip.spritesTransform.transform.eulerAngles.z);
        
        if (navigationShip.spritesTransform.transform.eulerAngles.z > 180)
            rotation = -90;
        else
            rotation = 90;
        
        // sequence.Join(
        //     navigationShip.spritesTransform.transform.DORotate(new Vector3(0f,0f,rotation), 3f)
        // );

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
            var from = AnimationCurvePosition(i, debugShip.eulerAngles.z+90f, debugShip.transform.position, transform.position);
            var to = AnimationCurvePosition(i+0.03f, debugShip.eulerAngles.z+90f, debugShip.transform.position, transform.position);
            Gizmos.DrawLine(from,to);
        }
        
        Gizmos.color = Color.cyan;
        
        Gizmos.DrawWireSphere(p2Debug, 0.1f);
        Gizmos.DrawWireSphere(p3Debug, 0.1f);

    }

    private Vector2 AnimationCurvePosition(float x, float startRotation, Vector2 startPos, Vector2 endPos)
    {
        Vector2 p1 = startPos;
        
        Vector2 p2 = startPos + new Vector2(Mathg.AngleToDirection(startRotation).x,Mathg.AngleToDirection(startRotation).y) * p2Lenght;
        p2Debug = p2;

        float side = (startPos.y < endPos.y) ? -1f : 1f;

        Vector2 p3 = endPos + Vector2.up * p3Lenght * side;
        p3Debug = p3;

        Vector2 p4 = endPos;

        var pos =Mathf.Pow(1f - x, 3f) * p1
                 + 3 * Mathf.Pow(1f - x, 2f) * x * p2
                 + 3 * (1f - x) * Mathf.Pow(x, 2f) * p3
                 + Mathf.Pow(x, 3f) * p4;

        return pos;
    }

    private float AnimationCurveRotation(float x, float startRotation, Vector2 startPos, Vector2 endPos)
    {
        var pos = AnimationCurvePosition(x, startRotation, startPos, endPos);
        var prevPos = AnimationCurvePosition(x - 0.1f, startRotation, startPos, endPos);

        return Mathg.AngleOfTheLineBetweenTwoPoints(prevPos, pos);
    }
    
}
