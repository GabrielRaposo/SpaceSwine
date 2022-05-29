using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Warper : MonoBehaviour
{
    const float VISUAL_SCALE = 1.2f;

    [SerializeField] float punchForce;
    [SerializeField] float punchDuration;

    [Header("References")]
    [SerializeField] Transform visualComponent;
    [SerializeField] ParticleSystem inParticleSystem;
    [SerializeField] ParticleSystem outParticleSystem;
    [SerializeField] AK.Wwise.Event activationAKEvent;

    Sequence sequence;
    Transform current;

    Warper other;

    public void Setup (Warper other)
    {
        this.other = other;
    }

    private void Start() 
    {
        Round round = GetComponentInParent<Round>();
        if (round)
            round.OnReset += ResetComponents;
    }

    private void ResetComponents()
    {
        visualComponent.localScale = Vector3.one * VISUAL_SCALE;

        if (inParticleSystem)
        {
            inParticleSystem.Stop();
            inParticleSystem.Clear();
        }

        if (outParticleSystem) 
        {
            outParticleSystem.Stop();
            outParticleSystem.Clear();
        }
    }

    private void OnTriggerStay2D (Collider2D collision) 
    {
        if (!other)
            return;

        if (current == collision.transform)
            return;

        // -- Alinha o vetor de direção com o plano XY, e compara o Y do vetor resultante
        Vector2 direction = transform.position - collision.transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        Vector2 anchoredDirection = RaposUtil.RotateVector(direction, angle);

        if (Mathf.Abs(anchoredDirection.y) > .1f)
            return;

        if (collision.CompareTag("Player")) 
        {
            LocalWarp (collision.transform);
            
            current = null;
            return;
        }

        CollectableThrowable collectable = collision.GetComponent<CollectableThrowable>();
        if (collectable)
        {
            collectable.NullifyPreviousHolder();
            LocalWarp (collision.transform);

            current = null;
            return;
        }
    }

    private void OnTriggerExit2D (Collider2D collision) 
    {
        if (collision.transform == current)
            current = null;
    }

    private void LocalWarp (Transform target)
    {
        other.CallWarp(target);
        
        PumpSequence();
        inParticleSystem?.Play();
        activationAKEvent?.Post(gameObject);
    }

    public void CallWarp (Transform target)
    {
        current = target;
        target.position = transform.position;
        
        PumpSequence();
        outParticleSystem?.Play();
    }

    private void PumpSequence()
    {
        if (!visualComponent)
            return;

        if (sequence != null)
            sequence.Kill();
        
        visualComponent.localScale = Vector3.one * VISUAL_SCALE;

        sequence = DOTween.Sequence();
        sequence.Append
        ( 
            visualComponent.DOPunchScale(punch: Vector2.one * punchForce, punchDuration)  
        );
    }
}
