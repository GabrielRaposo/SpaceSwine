using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BirdSpawner : MonoBehaviour
{
    const float GROUND_DISTANCE = .15f;    
    const float BASE_ANGLE = 45f;

    [SerializeField] Transform birdTransform;
    [SerializeField] SpriteRenderer mainRenderer;
    [SerializeField] float minFlightDistance;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve customCurve;
    [SerializeField] AK.Wwise.Event flightAKEvent;

    bool fleeToTheRight;
    Sequence mainSequence;
    Animator animator;
    CheckPlayerOnRadius checkPlayerOnRadius;

    enum State { Off, Landing, Idle, FlyAway }
    State state;

    private void Awake() 
    {
        animator = GetComponentInChildren<Animator>();    
    }

    void Start()
    {

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
                animator?.SetInteger("IdleState", 0);
                birdTransform.localPosition = Vector2.up * GROUND_DISTANCE;
                SetIdleLoop();
                checkPlayerOnRadius.enabled = true;
                break;

            case State.FlyAway:
                animator?.SetInteger("IdleState", -1);
                animator?.SetTrigger("FlyAway");
                SetFlyAwayLoop();
                checkPlayerOnRadius.enabled = false;
                break;

            case State.Off:
                gameObject.SetActive(false);
                checkPlayerOnRadius.enabled = false;
                break;

            default:
                checkPlayerOnRadius.enabled = false;
                break;
        }

        if (mainRenderer)
            mainRenderer.flipX = false;

        this.state = state;
    }

    private void SetIdleLoop()
    {
        /**
         Idle States:
          -1: OFF
           0: Stand
           1: Peck
           2: 360

         Animation States:
          0 - Wait
          1 - Peck
          2 - 360
          3 - Hops

         Always has chance% of: 
          Turn Around
    **/
        
        if (mainSequence != null)
            mainSequence.Kill();

        mainSequence = DOTween.Sequence();

        int randomRange = Random.Range(0, 4);
        switch (randomRange)
        {
            default: WaitSequence();  break;
            case 1:  PeckSequence();  break;
            case 2:  WeirdSequence(); break;
            case 3:  HopSequence();   break;
        }

        randomRange = Random.Range(0, 5);
        if (randomRange == 0 && mainRenderer)
            mainSequence.AppendCallback( () => mainRenderer.flipX = !mainRenderer.flipX );

        float floatRandomRange = Random.Range(.5f, .9f);
        mainSequence.AppendInterval(floatRandomRange);

        mainSequence.OnComplete( SetIdleLoop );
    }

    private void WaitSequence()
    {
        mainSequence.AppendCallback( () => animator?.SetInteger("IdleState", 0) );
        mainSequence.AppendInterval( .4f );
    }

    private void PeckSequence()
    {
        mainSequence.AppendCallback( () => animator?.SetInteger("IdleState", 1) );
        mainSequence.AppendInterval( 1.0f );
    }

    private void WeirdSequence()
    {
        mainSequence.AppendCallback( () => animator?.SetInteger("IdleState", 2) );
        mainSequence.AppendInterval( 1.0f );
    }

    private void HopSequence()
    {
        int randomRange = Random.Range(1,4);
        for (int i = 0; i < randomRange; i++)
        {
            mainSequence.Append
            ( 
                birdTransform.DOPunchPosition
                (
                    Vector2.up * .03f,
                    duration: .15f,
                    vibrato: 0,
                    elasticity: 1
                ) 
            );
        }
        mainSequence.AppendInterval( .3f );
    }

    private void SetFlyAwayLoop()
    {
        if (mainSequence != null)
            mainSequence.Kill();

        Vector2 startValue = birdTransform.localPosition;
        Vector2 endValue = birdTransform.localPosition;
        endValue += new Vector2(fleeToTheRight ? 1 : -1, 1) * minFlightDistance;

        if (flightAKEvent != null)
            flightAKEvent.Post(gameObject);

        mainSequence = DOTween.Sequence();

        mainSequence.Append
        (
            DOVirtual.Float(0.0f, 1.0f, duration, (t) => 
                {
                    float lerpedT = customCurve.Evaluate(t);
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

    private void OnDisable() 
    {
        if (mainSequence != null)
            mainSequence.Kill();
    }
}
