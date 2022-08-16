using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class GalaxyParallaxTester : MonoBehaviour
{
    [SerializeField] InputAction testInput;

    [Header("Values")]
    [SerializeField] float duration;

    [Header("Referencias")]
    [SerializeField] Transform starGroup;
    [SerializeField] Transform starParticles;
    [SerializeField] Transform bigStars;

    Sequence sequence;

    private void OnEnable() 
    {
        testInput.performed += (ctx) => PlaySequence();
        testInput.Enable();
    }

    private void Start() 
    {
        PlaySequence();
    }

    public void PlaySequence()
    {
        if (sequence != null)
            sequence.Kill();

        starGroup.transform.localPosition = Vector3.left * 4.0f;
        starParticles.transform.localPosition = Vector3.left * 3.0f;
        bigStars.transform.localPosition = Vector3.left * 9f;

        sequence = DOTween.Sequence();
        sequence.Append( starGroup.DOLocalMoveX(.5f, duration).SetEase(Ease.Linear) );
        sequence.Join( starParticles.DOLocalMoveX(3.0f, duration).SetEase(Ease.Linear) );
        sequence.Join( bigStars.DOLocalMoveX(0, duration).SetEase(Ease.Linear) );
    }

    private void OnDisable() 
    {
        if (sequence != null)
            sequence.Kill();

        testInput.Disable();
    }
}
