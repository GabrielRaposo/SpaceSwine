using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GuidingStar : MonoBehaviour
{
    [SerializeField] Gradient effectGradient;
    [SerializeField] float spinDuration;
    [SerializeField] int cycles;
    [SerializeField] ParticleSystem mainPS;

    Sequence sequence;
    SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.eulerAngles = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (mainPS)
            mainPS.Play();

        if (sequence != null)
            sequence.Kill();

        transform.eulerAngles = Vector3.zero;
        
        sequence = DOTween.Sequence();
        sequence.Append( transform.DORotate(Vector3.forward * 360f * cycles, spinDuration, RotateMode.FastBeyond360) )
            .SetEase(Ease.OutCirc);
        sequence.Join
        (
            DOVirtual.Float
            (
                from: 0, to: cycles * 2, spinDuration, 
                (t) => spriteRenderer.color = effectGradient.Evaluate(t % 1)
            )
        );

    }
}
