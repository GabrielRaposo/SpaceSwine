using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SetSceneNavigationObject : NavigationObject
{
    public BuildIndex scene = BuildIndex.World1Exploration;
    public SpriteRenderer selector;
    
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
        
        sequence.Append(
            navigationShip.transform.DOMove(transform.position+new Vector3(0f,0.6f,0f), 3f)
                .SetEase(Ease.OutCubic)
        );

        float rotation;

        Debug.Log("euler z:" + navigationShip.spritesTransform.transform.eulerAngles.z);
        
        if (navigationShip.spritesTransform.transform.eulerAngles.z > 180)
            rotation = -90;
        else
            rotation = 90;
        
        sequence.Join(
            navigationShip.spritesTransform.transform.DORotate(new Vector3(0f,0f,rotation), 3f)
        );

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
    
}
