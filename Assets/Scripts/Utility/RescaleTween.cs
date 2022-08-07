using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RescaleTween : MonoBehaviour
{
    [SerializeField] Vector2 startScale;
    [SerializeField] Vector2 targetScale;
    [SerializeField] float duration;

    void Start()
    {
        transform.localScale = startScale;
    }

    public void Call()
    {
        transform.DOScale(targetScale, duration);
    }
}
