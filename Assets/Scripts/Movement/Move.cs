using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Move : MonoBehaviour
{
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float duration;
    [SerializeField] bool stay;
    private bool stop;

    int direction;
    float count;

    private float distance;
    private Vector3 normalizedDirection;

    private Round _round;
    private Vector3 startPosition;

    [SerializeField] private bool keepTrackOfChildren;
    private List<Vector3> childrenPosition;

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

        if (keepTrackOfChildren)
        {
            childrenPosition = new List<Vector3>();

            for (int i = 0; i < transform.childCount; i++)
            {
                childrenPosition.Add(transform.GetChild(i).localPosition);
            }
            
        }
    }

    private void Reset()
    {
        direction = 1; 
        count = 0;

        stop = false;
        
        transform.localPosition = startPosition;
        count = 0f;

        if (keepTrackOfChildren)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).localPosition = childrenPosition[i];
            }
        }
    }

    private void FixedUpdate() 
    {
        if(stop) return;
        
        float step = (distance / duration) * direction * Time.fixedDeltaTime;
        transform.localPosition += normalizedDirection * step;

        count += Time.fixedDeltaTime;
        if (count >= duration)
        {
            if (stay)
            {
                stop = true;
                return;
            }
                
            direction *= -1;
            count = 0;
        }
    }
}
