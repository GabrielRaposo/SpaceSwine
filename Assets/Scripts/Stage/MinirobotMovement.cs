using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MinirobotMovement : AttachToPlanet
{
    [Header("Movement options")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private float pauseTime;
    [SerializeField] private float duration;
    [SerializeField] private float range;

    Sequence mySequence;
    float startingAngle;
    bool initialized;


    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;

        if (!initialized)
        {
            startingAngle = angle;
            initialized = true;
        }

        if (duration == 0 || range == 0)
            return;
        
        this.DOKill();

        angle = startingAngle;
        sprite.flipX = range>0?false:true;

        mySequence = DOTween.Sequence();

        //Debug.Log("mySequence.id: " + mySequence.id);

        mySequence.AppendCallback(()=>SetMoveAnimation(true));
        //var going = DOTween.To(() => angle, x => angle = x, angle + range, duration).SetEase(Ease.Linear);
        //s.Append( DOTween.To(() => angle, x => angle = x, startingAngle + range, duration).SetEase(Ease.Linear) );
        mySequence.Append( DOVirtual.Float(from: startingAngle, to: startingAngle + range, duration, f => angle = f ).SetEase(Ease.Linear) );
        mySequence.AppendCallback(()=>SetMoveAnimation(false));
        mySequence.AppendInterval(pauseTime);
        mySequence.AppendCallback(() => sprite.flipX = range>0?true:false);

        mySequence.AppendCallback(()=>SetMoveAnimation(true));
        //var returning = DOTween.To(() => angle, x => angle = x, angle, duration).SetEase(Ease.Linear);
        //s.Append( DOTween.To(() => angle, x => angle = x, startingAngle, duration).SetEase(Ease.Linear) );
        mySequence.Append( DOVirtual.Float(from: startingAngle + range, to: startingAngle, duration, f => angle = f ).SetEase(Ease.Linear) );
        mySequence.AppendCallback(()=>SetMoveAnimation(false));
        mySequence.AppendInterval(pauseTime);
        mySequence.AppendCallback(() => sprite.flipX = range>0?false:true);
        
        mySequence.SetLoops(-1);
    }

    private void OnDisable() 
    {
        if (!Application.isPlaying)
            return;

        if (mySequence != null)
            mySequence.Kill();

        this.DOKill();
    }

    protected override void Update()
    {
        Attach();
    }

    private void SetMoveAnimation(bool value)
    {
        //Debug.Log("SetMoveAnimation: " + value);
        if (animator != null)
            animator.SetBool("Moving", value);
    }
}
