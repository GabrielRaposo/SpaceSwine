using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class SpaceBooster : MonoBehaviour
{
    [SerializeField] Vector2[] launchDirections;    
    [SerializeField] float cooldownDuration;

    [Header("References")]
    [SerializeField] Transform visualComponent;
    [SerializeField] Transform rotationAnchor;
    [SerializeField] ParticleSystem spinParticleSystem;
    [SerializeField] AK.Wwise.Event activationAKEvent;

    int index;
    bool interactable = true;
    Sequence sequence;
    Animator animator;

    private void OnValidate() 
    {
        if (Application.isPlaying)
            return;

        SetRotationAnchor();
    }

    private void Start() 
    {   
        animator = GetComponent<Animator>();

        Round round = GetComponentInParent<Round>();
        if (round)
            round.OnReset += ResetComponents;
    }

    private void ResetComponents()
    {
        StopAllCoroutines();

        if (sequence != null)
            sequence.Kill();

        if (spinParticleSystem) 
        {
            spinParticleSystem.Stop();
            spinParticleSystem.Clear();
        }

        index = 0;
        SetRotationAnchor();
        interactable = true;
        visualComponent.DOKill();
        visualComponent.localScale = Vector3.one;
        if (activationAKEvent != null && activationAKEvent.IsPlaying(gameObject))
            activationAKEvent.Stop(gameObject);
        animator.SetTrigger("Reset");
    }

    private Vector2 GetLaunchDirection()
    {
        if (launchDirections == null || launchDirections.Length < 1)
            return Vector2.up;

        return launchDirections[index % launchDirections.Length];
    }

    private void SetRotationAnchor()
    {
        if (rotationAnchor == null)
            return;

        rotationAnchor.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.right, GetLaunchDirection().normalized);
    }

    private void SetRotationAnchorAnimated()
    {
        if(rotationAnchor == null)
            return;

        Vector3 rotation = Vector3.forward * Vector2.SignedAngle(Vector2.right, GetLaunchDirection().normalized);

        rotationAnchor.DORotate(rotation, 0.35f);

    }

    public void SetFirstLaunchDirection(float x, float y)
    {
        if (launchDirections.Length == 0)
        {
            launchDirections = new[] { new Vector2(x,y) };
            return;
        }

        launchDirections[0] = new Vector2(x, y);
        
        if(Application.isPlaying)
            SetRotationAnchorAnimated();
        else
            OnValidate();
    }

    private void OnTriggerStay2D (Collider2D collision) 
    {
        if (!interactable)
            return;

        // Alinha o vetor de direção com o plano XY, e compara o Y do vetor resultante
        Vector2 direction = transform.position - collision.transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        Vector2 anchoredDirection = RaposUtil.RotateVector(direction, angle);

        //Debug.DrawLine(transform.position, transform.position + (Vector3) direction, Color.red, 3f);
        if (Mathf.Abs(anchoredDirection.y) > .1f)
            return;

        if (collision.CompareTag("Player"))
        {
            SpaceJumper spaceJumper = collision.GetComponent<SpaceJumper>();
            if (!spaceJumper)
                return;

            PlayerLaunch(spaceJumper);

            return;
        }

        CollectableThrowable collectable = collision.GetComponent<CollectableThrowable>();
        if (collectable)
        {
            Rigidbody2D rb = collectable.GetComponent<Rigidbody2D>();
            if (!rb)
                return;

            CollectableLaunch(collectable, rb);
        }
    }

    protected virtual void CollectableLaunch(CollectableThrowable collectable, Rigidbody2D rb)
    {
        collectable.NullifyPreviousHolder();
        collectable.transform.position = transform.position;
        rb.velocity = GetLaunchDirection().normalized * rb.velocity.magnitude;

        StartCoroutine(CooldownRoutine());
    }

    protected virtual void PlayerLaunch(SpaceJumper spaceJumper)
    {
        spaceJumper.transform.position = transform.position;
        spaceJumper.RedirectIntoDirection(GetLaunchDirection().normalized);

        StartCoroutine(CooldownRoutine());
    }

    IEnumerator CooldownRoutine()
    {
        spinParticleSystem?.Play();
        activationAKEvent?.Post(gameObject);
        visualComponent.DOPunchScale(Vector3.one * .1f, .2f, vibrato: 0);

        SetSpinState(true);
        interactable = false;
        
        yield return new WaitForSeconds(cooldownDuration);

        SetSpinState(false);
        interactable = true;
    }

    private void SetSpinState(bool value)
    {
        animator.SetTrigger(value ? "Spin" : "Reset");
    }

    public void ChangeLaunchDirection (float duration, UnityAction afterSequenceAction)
    {
        index++;
        index %= launchDirections.Length;

        float angle = Vector2.SignedAngle(Vector2.right, GetLaunchDirection().normalized);
        if (angle < 0)
            angle += 360;

        if (sequence != null)
            sequence.Kill();
        sequence = DOTween.Sequence();

        sequence.Append
        (
            rotationAnchor.DORotate
            (
                angle * Vector3.forward, 
                duration, 
                RotateMode.Fast
            )    
        );
        
        sequence.OnComplete
        (
            () => 
            {
                SetRotationAnchor();             
                afterSequenceAction?.Invoke();
            }
        );
    }

    private void OnDisable() 
    {
        if (activationAKEvent != null && activationAKEvent.IsPlaying(gameObject))
            activationAKEvent.Stop(gameObject);
    }

    private void OnDrawGizmos() 
    {
            
    }
}
