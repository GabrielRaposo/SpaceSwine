using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomMove))]
public class Move_AddDelayOnEvent : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] CustomMove.EventEnum eventType;

    void Start()
    {
        CustomMove customMove = GetComponent<CustomMove>();

        switch (eventType)
        {
            case CustomMove.EventEnum.OnStepReached:
                customMove.OnStepReached = DelayEvent;
                break;

            case CustomMove.EventEnum.OnLoopCompleted:
                customMove.OnLoopCompleted = DelayEvent;
                break;

            case CustomMove.EventEnum.OnDirectionInverted:
                customMove.OnDirectionInverted = DelayEvent;
                break;
        }
    }

    private void DelayEvent()
    {
        CustomMove customMove = GetComponent<CustomMove>();
        customMove.SetHold(duration);
    }
}
