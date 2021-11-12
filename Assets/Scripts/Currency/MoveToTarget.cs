using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveToTarget : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] AnimationCurve curve;
    [SerializeField] bool turnOffOnReach;

    float t;
    Vector2 startingPosition;

    Transform target;
    UnityAction OnReachTarget;

    public void SetTarget(Transform target, UnityAction OnReachTarget)
    {
        this.target = target;
        this.OnReachTarget = OnReachTarget; 

        startingPosition = transform.position;
        t = 0;
        enabled = true;
    }

    void Update()
    {
        if (!target)
            return;

        t += Time.deltaTime;
        if (t > duration)
            t = duration;

        float a = t * curve.Evaluate(t / duration);

        transform.position = Vector2.Lerp(startingPosition, target.position, a / duration);

        if (t == duration)
        {
            OnReachTarget?.Invoke();
            if (turnOffOnReach)
            {
                target = null;
                enabled = false;
            }
        }
    }
}
