using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeatToy : MonoBehaviour
{
    const float MAX_VALUE = 100f;

    [SerializeField] Vector2 targetPosition;
    [SerializeField] float slideDuration;

    [Header("Sound Events")]
    [SerializeField] AK.Wwise.Event hitSound1AKEvent;
    [SerializeField] AK.Wwise.Event hitSound2AKEvent;
    [SerializeField] AK.Wwise.Event hitSound3AKEvent;

    [Header("Sidechain")]
    [SerializeField] AK.Wwise.RTPC mainVolumeParam;
    [SerializeField] AnimationCurve sidechainCurve;
    [SerializeField] float sidechainDuration;
    [SerializeField] float auxValue;

    bool selected;
    float sidechainCount;
    Vector2 startingPosition;

    Sequence slideSequence;

    private void Awake() 
    {
        startingPosition = transform.position;    
    }

    void Start()
    {
        sidechainCount = -1;
    }

    public void Show (bool value)
    {
        if (selected == value)
            return;

        selected = value;

        if (slideSequence != null)
            slideSequence.Kill();

        {   

        }

        slideSequence = DOTween.Sequence();
        slideSequence.Append
        (
            transform.DOMove
            (
                value ? targetPosition : startingPosition,
                slideDuration
            ).SetEase(Ease.OutCirc)
        );
    }

    public void CallHitSound(int index)
    {
        return;
        switch (index)
        {
            case 1:
                hitSound1AKEvent?.Post(gameObject);
                break;

            case 2:
                hitSound2AKEvent?.Post(gameObject);
                break;

            case 3:
                hitSound3AKEvent?.Post(gameObject);
                sidechainCount = sidechainDuration;
                break;
        }
    }

    private void Update() 
    {
        if (sidechainCount < 0)
            return;

        if (mainVolumeParam == null)
            return;

        float t = 1 - (sidechainCount / sidechainDuration);
        mainVolumeParam.SetGlobalValue( sidechainCurve.Evaluate(t) * MAX_VALUE );

        sidechainCount -= Time.deltaTime;
        if (sidechainCount < 0)
        {
            mainVolumeParam.SetGlobalValue(MAX_VALUE);
            sidechainCount = -1;
        }
    }
}
