using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricMindToggler : MonoBehaviour
{
    [SerializeField] private float delay;
    [Space]
    [SerializeField] float duration1;
    [SerializeField] float duration2;

    ElectricMind electricMind;    

    float durationCount;
    bool timingState;

    void Start()
    {
        if (duration1 <= 0 || duration2 <= 0)
            return;

        electricMind = GetComponent<ElectricMind>();
        if (!electricMind)
            return;

        Setup();

        Round round = GetComponentInParent<Round>();
        if (round)
        {
            round.OnReset += Setup;
        }
    }

    private void Setup()
    {
        timingState = true;
        ResetDuration();
        durationCount += delay;
    }

    private void FixedUpdate() 
    {
        durationCount -= Time.fixedDeltaTime;
        if (durationCount < 0)
        {
            if (electricMind)
                electricMind.ToggleActivation();

            timingState = !timingState;
            ResetDuration();
        }
    }

    private void ResetDuration()
    {
        durationCount = timingState ? duration1 : duration2;
    }
}
