using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DialogueAnimation : MonoBehaviour
{
    [SerializeField] Transform visualComponent;
    
    [Header("Values")]
    [SerializeField] Vector2 resizeRatio; 
    [SerializeField] float duration;

    Vector3 initialScale;
    bool moving;

    private void Awake() 
    {
        initialScale = transform.localScale;
    }

    public void ReceiveString (string s)
    {
        if (moving || !visualComponent)
            return;

        if (s.Contains("."))
            return;
        
        moving = true;
        visualComponent.DOPunchScale(Vector3.one * resizeRatio, duration)
            .OnComplete(() => moving = false);
    }

    public void ResetState()
    {
        if (!visualComponent)
            return;

        visualComponent.DOKill();
        transform.localScale = initialScale;
        moving = false;
    }
}
