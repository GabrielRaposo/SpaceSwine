using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BirdSpawner : MonoBehaviour
{
    const float GROUND_DISTANCE = .15f;    
    const float BASE_ANGLE = 45f;

    [SerializeField] Transform birdTransform;
    [SerializeField] float minFlightDistance;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve customCurve;

    bool fleeToTheRight;
    Sequence mainSequence;
    Animator animator;
    CheckPlayerOnRadius checkPlayerOnRadius;

    enum State { Off, Landing, Idle, FlyAway }
    State state;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        checkPlayerOnRadius = GetComponent<CheckPlayerOnRadius>();
        if (checkPlayerOnRadius)
            checkPlayerOnRadius.PlayerOnRadiusEvent += OnPlayerOnRadius;

        SetState (State.Idle);
    }

    private void SetState (State state)
    {
        switch (state)
        {
            case State.Idle:
                birdTransform.localPosition = Vector2.up * GROUND_DISTANCE;
                SetIdleLoop();
                checkPlayerOnRadius.enabled = true;
                break;

            case State.FlyAway:
                SetFlyAwayLoop();
                checkPlayerOnRadius.enabled = false;
                break;

            case State.Off:
                if (mainSequence != null)
                    mainSequence.Kill();

                gameObject.SetActive(false);
                break;

            default:
                checkPlayerOnRadius.enabled = false;
                break;
        }

        this.state = state;
    }

    private void SetIdleLoop()
    {
        
    }

    private void SetFlyAwayLoop()
    {
        if (mainSequence != null)
            mainSequence.Kill();

        Vector2 startValue = birdTransform.localPosition;
        Vector2 endValue = birdTransform.localPosition;
        endValue += new Vector2(fleeToTheRight ? 1 : -1, 1) * minFlightDistance;

        mainSequence = DOTween.Sequence();

        //mainSequence.Append
        //( 
        //    birdTransform.DOLocalMove ( endValue, duration )
        //        .SetEase(Ease.Linear)
        //);

        mainSequence.Append
        (
            DOVirtual.Float(0.0f, 1.0f, duration, (t) => 
                {
                    float lerpedT = t * customCurve.Evaluate(t);
                    birdTransform.localPosition = Vector2.Lerp(startValue, endValue, lerpedT);
                }
            )
        );
        mainSequence.OnComplete( () => SetState(State.Off) );
    }

    private void OnPlayerOnRadius (bool onRight)
    {
        if (state != State.Idle)
            return;

        fleeToTheRight = !onRight;
        SetState (State.FlyAway);
    }

}
