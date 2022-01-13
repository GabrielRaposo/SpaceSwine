using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Move : MonoBehaviour
{
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float duration;
    [SerializeField] bool stay;

    int direction;
    float count;

    private float distance;
    private Vector3 normalizedDirection;

    private Round _round;
    private Vector3 startPosition;
    
    void Start()
    {
        direction = 1; 
        count = 0;

        if (duration == 0)
            duration = float.MinValue;
        
        distance = ( targetPosition - transform.localPosition).magnitude;
        normalizedDirection = (targetPosition - transform.localPosition).normalized;

        startPosition = transform.localPosition;

        _round = GetComponentInParent<Round>();

        _round.OnReset += Reset;
    }

    private void Reset()
    {
        transform.localPosition = startPosition;
        count = 0f;
    }

    private void FixedUpdate() 
    {
        float step = (distance / duration) * direction * Time.fixedDeltaTime;
        transform.localPosition += normalizedDirection * step;

        count += Time.fixedDeltaTime;
        if (count >= duration)
        {
            if (stay)
            {
                this.enabled = false;
                return;
            }
                
            direction *= -1;
            count = 0;
        }
    }
}
