using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class ScreenShakeCaller : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] float strength;
    [SerializeField] int vibrato = 10;
    [SerializeField] float randomness = 90;
    [SerializeField] Transform cameraTransform;

    Vector3 startingPosition;
    Sequence sequence;

    void Start()
    {
        if (!cameraTransform)
            return;

        startingPosition = cameraTransform.position;

        //RaposUtil.WaitSeconds(this, duration: 2f, CallScreenshake);
    }

    public void CallScreenshake()
    {
        if (sequence != null)
            sequence.Kill();

        cameraTransform.position = startingPosition;

        sequence = DOTween.Sequence();
        sequence.Append
        (
            cameraTransform.DOShakePosition
            (
                duration,
                strength,
                vibrato,
                randomness,
                snapping: false,
                fadeOut: true
            )
        );

        sequence.OnComplete 
        (
            () => 
            {
                cameraTransform.position = startingPosition;
            }
        );
    }
}
