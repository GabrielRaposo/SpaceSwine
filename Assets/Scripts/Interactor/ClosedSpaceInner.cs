using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClosedSpaceInner : MonoBehaviour
{
    [SerializeField] float transitionDuration;

    [Header("Preferences")]
    [SerializeField] SpriteRenderer visualComponent;
    [SerializeField] GravitationalPlatform gravitationalPlatform;

    Sequence sequence;

    void Start()
    {
        //SetState(false);

        visualComponent.enabled = false;
        gravitationalPlatform.gameObject.SetActive(false);        
    }

    public void SetState (bool value)
    {
        gravitationalPlatform.gameObject.SetActive(value);

        if (sequence != null)
            sequence.Kill();

        visualComponent.color = new Color (1, 1, 1, value ? 0 : 1);
        if (value)
            visualComponent.enabled = true;
        
        sequence = DOTween.Sequence();
        sequence.Append
        (
            visualComponent.DOFade(value ? 1 : 0, transitionDuration)
                .SetEase(Ease.Linear)
        );

        if (!value)
            sequence.OnComplete( () => visualComponent.enabled = false );
    }

    public GravitationalBody GetGravitationalBody
    {
        get 
        {
            if (!gravitationalPlatform)
                return null;

            return gravitationalPlatform;
        }
    }

    private void OnDisable() 
    {
        if (visualComponent)
            visualComponent.DOKill();
    }
}
