using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MinirobotMovment : AttachToPlanet
{
    [Header("Movement options")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private float pauseTime;
    [SerializeField] private float duration;
    [SerializeField] private float range;

    
    private void OnEnable()
    {
        if(duration == 0 || range == 0)
            return;
        
        var s = DOTween.Sequence();

        s.AppendCallback(()=>SetMoveAnimation(true));
        var going = DOTween.To(() => angle, x => angle = x, angle + range, duration).SetEase(Ease.Linear);
        s.Append(going);
        s.AppendCallback(()=>SetMoveAnimation(false));
        s.AppendInterval(pauseTime);
        s.AppendCallback(() => sprite.flipX = true);

        s.AppendCallback(()=>SetMoveAnimation(true));
        var returning = DOTween.To(() => angle, x => angle = x, angle, duration).SetEase(Ease.Linear);
        s.Append(returning);
        s.AppendCallback(()=>SetMoveAnimation(false));
        s.AppendInterval(pauseTime);
        s.AppendCallback(() => sprite.flipX = false);
        
        s.SetLoops(-1);
    }

    protected override void Update()
    {
        Attach();
    }

    private void SetMoveAnimation(bool value)
    {
        if (animator != null)
            animator.SetBool("Moving", value);
    }
}
