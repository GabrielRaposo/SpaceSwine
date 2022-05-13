using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class SpaceBooster : MonoBehaviour
{
    [SerializeField] Vector2[] launchDirections;    
    [SerializeField] float cooldownDuration;

    [Space(5)]

    [SerializeField] Transform rotationAnchor;

    int index;
    bool interactable = true;
    Sequence sequence;
    Animator animator;

    private void Start() 
    {   
        animator = GetComponent<Animator>();    
    }

    private void OnValidate() 
    {
        if (Application.isPlaying)
            return;

        SetRotationAnchor();
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

    private void OnTriggerStay2D (Collider2D collision) 
    {
        if (!interactable)
            return;

        // Alinha o vetor de direção com o plano XY, e compara o Y do vetor resultante
        Vector2 direction = transform.position - collision.transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        Vector2 anchoredDirection = RaposUtil.RotateVector(direction, angle);

        //Debug.Log("anchoredDirection: " + anchoredDirection.y);
        Debug.DrawLine(transform.position, transform.position + (Vector3) direction);
        if (anchoredDirection.y > .1f)
            return;

        //Debug.Log("passed");

        if (collision.CompareTag("Player"))
        {
            SpaceJumper spaceJumper = collision.GetComponent<SpaceJumper>();
            if (!spaceJumper)
                return;

            spaceJumper.transform.position = transform.position;
            spaceJumper.RedirectIntoDirection (GetLaunchDirection().normalized);

            StartCoroutine( CooldownRoutine() );

            return;
        }

        CollectableThrowable collectable = collision.GetComponent<CollectableThrowable>();
        if (collectable)
        {
            Rigidbody2D rb = collectable.GetComponent<Rigidbody2D>();
            if (!rb)
                return;

            collectable.NullifyPreviousHolder();
            rb.velocity = GetLaunchDirection().normalized * rb.velocity.magnitude;

            StartCoroutine( CooldownRoutine() );
        }
    }

    IEnumerator CooldownRoutine()
    {
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

        if (sequence != null)
            sequence.Kill();
        sequence = DOTween.Sequence();

        sequence.Append
        (
            rotationAnchor.DORotate
            (
                Vector2.SignedAngle(Vector2.right, GetLaunchDirection().normalized) * Vector3.forward, 
                duration, 
                RotateMode.FastBeyond360
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
}
